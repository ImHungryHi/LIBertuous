#include "StdAfx.h"
#include "EdgeMatching.h"
#include <stdio.h>
#include <tchar.h>

#include <cstdio>
#include <cstdlib>
#include <iostream>
#include <string>
#include <fstream>
#include <iterator>
#include <sstream>


EdgeMatching::EdgeMatching(cl::Platform platform, cl::Device device, cl::Context context)
{
	EdgeMatching::platform = platform;
	EdgeMatching::device = device;
	EdgeMatching::context = context;
	EdgeMatching::computingTime = 0;
}

EdgeMatching::EdgeMatching(void)
{
}
EdgeMatching::~EdgeMatching(void)
{
}

//get the computing time
double EdgeMatching::getComputingTimeEM() {
	return computingTime;
}


// init data in arrays
void EdgeMatching::initDataEM() {
	std::ifstream infile("pieces.txt");
	std::string line;
	int i=-1;

	while (std::getline(infile, line))
	{
		std::istringstream iss(line);
		if(i == -1) {
			//get first line (w*h)
			int w, h;
			if (!(iss >> w >> h)) { 
				break; 
			}
			i++;
		}
		else {
			int a, b, c, d;
			if (!(iss >> a >> b >> c >> d)) { 
				break; 
			} // error
			if(a==0 && b==0) {
				cout << a << b << c << d << "Cornerpiece\n";
			}
			else if(a==0) {
				cout << a << b << c << d << "Edge\n";
			}
			else {
				cout << a << b << c << d << "Rest\n";
			}
			arrayPieces[i*4] = a;
			arrayPieces[i*4+1] = b;
			arrayPieces[i*4+2] = c;
			arrayPieces[i*4+3] = d;
			i++;
		}
	}
	for(int i = 0; i < (49*4); i++) {
		cout << arrayPieces[i];
	}
	cout << "-----";
}


//executes the kernel
void EdgeMatching::setArgsAndExecuteKernelEM(Kernel kernel){
	cl_int err = CL_SUCCESS;  //holds error values

	//create open cl memory buffers
	Buffer piecesBuffer = Buffer(context, CL_MEM_READ_ONLY | CL_MEM_COPY_HOST_PTR, ARRAY_SIZE * sizeof(short)*4, (void *) &arrayPieces[0]);

	//Outputbuffer
	Buffer outputBuffer = Buffer(context, CL_MEM_WRITE_ONLY | CL_MEM_USE_HOST_PTR, ARRAY_SIZE * sizeof(short)*4, (void *) &arraySolution[0]);
	Buffer outputSolutionBuffer = Buffer(context, CL_MEM_WRITE_ONLY | CL_MEM_USE_HOST_PTR, ARRAY_SIZE * sizeof(short)*2, (void *) &solutionPositions[0]);

	//set kernel arguments
	kernel.setArg(0, piecesBuffer);
	kernel.setArg(1, outputBuffer);
	kernel.setArg(2, outputSolutionBuffer);


	//creates a CommandQueue(context, device, properties, err)
    CommandQueue queue(context, device, 0, &err);
	util::check_error(err, "CommandQueue::CommandQueue() failed"); 	//if not succeeded, print error and exit

	cout << "# Running OPENCL program" << endl;
	//do the work
	//cl::CommandQueue::enqueueNDRangeKernel(kernel, offset, global, local, events = null, event = null)
	err = queue.enqueueNDRangeKernel( kernel, cl::NullRange, cl::NDRange(ARRAY_SIZE), cl::NullRange);

	util::check_error(err, "CommandQueue::enqueueNDRangeKernel() failed"); 	//if not succeeded, print error and exit

	//map cBuffer to host pointer, this enforces a sync with the host backing space
	int *output = (int *) queue.enqueueMapBuffer(outputBuffer, CL_TRUE , CL_MAP_READ, 0, ARRAY_SIZE*4 / sizeof(int), NULL, NULL, &err);
	int *solution = (int *) queue.enqueueMapBuffer(outputSolutionBuffer, CL_TRUE , CL_MAP_READ, 0, ARRAY_SIZE*2 / sizeof(int), NULL, NULL, &err);
	util::check_error(err, "CommandQueue::enqueueMapBuffer() failed"); 	//if not succeeded, print error and exit



	//print output array
	cout << "printing output buffer:" << endl;
	cout << "+--------------------------------------------------------------+\n";
	for(int i = 0; i < HEIGHT; i++) {
		cout << "|    ";
		for(int j = 0; j < WIDTH; j++) {
			cout << arraySolution[(i*7+j)*4 + 2] << "   |    ";
		}
		cout << "\n|";
		for(int j = 0; j < WIDTH; j++) {
			cout << arraySolution[(i*7+j)*4 + 1] << "      " << arraySolution[(i*7+j)*4 + 3] << "|";
		}
		cout << "\n|";
		cout << "    ";
		for(int j = 0; j < WIDTH; j++) {
			cout << arraySolution[(i*7+j)*4] << "   |    ";
		}
		cout << "\n";
		cout << "+--------------------------------------------------------------+\n";
	}
	//Print the solutionarray sequential
	for (int i = 0; i < (ARRAY_SIZE*2); i++) {
		cout << solutionPositions[i] << " ";
		if(i%7 == 6) cout << endl;
	}
	


	//release our hold on accessing the memory
	err = queue.enqueueUnmapMemObject(outputBuffer, (void *) output);
	util::check_error(err, "CommandQueue::enqueueUnmapMemObject() failed"); 	//if not succeeded, print error and exit
}




//main program
void EdgeMatching::executeEdgeMatching() {
	cout << "----------------------------------" << endl;
	cout << "EDGMATCHING FOR A PUZZLE OF 7 BY 7" << endl;
	initDataEM();

	//Call the kernel
	Kernel kernel = util::loadAndBuildKernelFromFile(context, "Edge.cl", "edge"); 

	clock_t startTime = clock();
	//execute kernel
	setArgsAndExecuteKernelEM(kernel);
	clock_t stopTime = clock();

	computingTime = util::showComputingTime(stopTime, startTime);
}



