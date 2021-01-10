#include "StdAfx.h"
#include "TSP.h"



//constructor
TSP::TSP(cl::Platform platform, cl::Device device, cl::Context context) {
	TSP::platform = platform;
	TSP::device = device;
	TSP::context = context;
	TSP::computingTime = 0;
}

//default construct / destruct
TSP::TSP(void) {}
TSP::~TSP(void) {}


//get the computing time
double TSP::getComputingTime() {
	return computingTime;
}



// TSP: init data in arrays
void TSP::initData() {
	for (int i = 0; i < NUMCITIES; i++) {
		cities[i] = i; // give each city a number from 0 to maxinum number of cities -1
		results[i] = 0;
	}
	results[NUMCITIES] = 0; //also clean forelast field in this array
	results[NUMCITIES+1] = 100000; //set the last field in this array very high because this is where the shortest distance will be kept

	//generate random cities
    cout << "generating locations" << endl;
    for (int i = 0; i < NUMCITIES; i++) {
        cout << "Location (x,y):  ";
		coordinates[i] = (rand() % (100+1-0));
		coordinates[i+NUMCITIES] = (rand() % (100+1-0));
        cout << coordinates[i] << "   " << coordinates[i+NUMCITIES] << endl;
    }
}





// calculates the travelling salesman problem on the gpu
void TSP::executeTsp() {
	cout << "----------------------------------" << endl;
	cout << "TSP FOR " << NUMCITIES << " CITIES" << endl;
	initData();

	//@runtime load/compile kernel source file
	Kernel kernel = util::loadAndBuildKernelFromFile(context, "TSPkernel.cl", "tsp");

	clock_t startTime = clock();
	//set kernel arguments and execute!
	setArgsAndExecuteKernel(kernel);
	clock_t stopTime = clock();

	//print solution
	cout << "path:" << endl;
	for (int i = 0; i <= NUMCITIES; i++) cout << results[i] << " ";
	cout << endl << "distance: " << results[NUMCITIES+1] << endl;
	computingTime = util::showComputingTime(stopTime, startTime);
}



//executes the kernel
void TSP::setArgsAndExecuteKernel(Kernel kernel){
	cl_int err = CL_SUCCESS;  //holds error values

	//create open cl memory buffers
	Buffer cities_buffer = Buffer(context, CL_MEM_READ_ONLY | CL_MEM_COPY_HOST_PTR, NUMCITIES * sizeof(int), (void *) &cities[0]);
	Buffer coördinates_buffer = Buffer(context, CL_MEM_READ_ONLY | CL_MEM_COPY_HOST_PTR, NUMCITIES * sizeof(int) * 2, (void *) &coordinates[0]);
	Buffer return_buffer = Buffer(context, CL_MEM_WRITE_ONLY | CL_MEM_USE_HOST_PTR, (NUMCITIES+2) * sizeof(int), (void *) &results[0]);

	//set kernel arguments
	cout << "# Setting kernel arguments..." << endl;
	kernel.setArg(0, cities_buffer);
	kernel.setArg(1, coördinates_buffer);
	kernel.setArg(2, return_buffer);
	//the amount of cities is also defined inside the kernel, so no need to pass it as argument

	//creates a CommandQueue(context, device, properties, err)
    CommandQueue queue(context, device, 0, &err);
	util::check_error(err, "CommandQueue::CommandQueue() failed"); 	//if not succeeded, print error and exit

	cout << "# Enqueue OPENCL program and mapping buffers" << endl;
	err = queue.enqueueNDRangeKernel( kernel, cl::NullRange, cl::NDRange(NUMCITIES-1, NUMCITIES-1), cl::NullRange);
	util::check_error(err, "CommandQueue::enqueueNDRangeKernel() failed"); 	//if not succeeded, print error and exit

	cout << "# Running OPENCL program" << endl << " ... busy" << endl;
	//map cBuffer to host pointer, this enforces a sync with the host backing space
	int *output = (int *) queue.enqueueMapBuffer(return_buffer, CL_TRUE /*= block*/, CL_MAP_READ, 0, (NUMCITIES+2) * sizeof(int), NULL, NULL, &err);
	util::check_error(err, "CommandQueue::enqueueMapBuffer() failed"); 	//if not succeeded, print error and exit

	cout << "# Executing kernel completed!" << endl << endl;
	//release our hold on accessing the memory
	err = queue.enqueueUnmapMemObject(return_buffer, (void *) output);
	util::check_error(err, "CommandQueue::enqueueUnmapMemObject() failed"); 	//if not succeeded, print error and exit
}