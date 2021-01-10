#pragma once
class EdgeMatching
{
private:
	double computingTime;

	//OpenCL data
	cl::Platform platform;
	cl::Device device;
	cl::Context context;

	// Data for edgematching class
	#define ARRAY_SIZE 49
	#define HEIGHT 7
	#define WIDTH 7
	short arrayPieces[ARRAY_SIZE*4];
	short arraySolution[ARRAY_SIZE*4]; //Full board as solution
	short solutionPositions[ARRAY_SIZE*2]; //Placement of the pieces in arrayPieces /1 = piece 
	//private functions
	void initDataEM(void);
	void setArgsAndExecuteKernelEM(cl::Kernel kernel);

public:
	EdgeMatching(cl::Platform platform, cl::Device device, cl::Context context);
	EdgeMatching(void);
	~EdgeMatching(void);
	void executeEdgeMatching(void);
	double getComputingTimeEM(void);
};


