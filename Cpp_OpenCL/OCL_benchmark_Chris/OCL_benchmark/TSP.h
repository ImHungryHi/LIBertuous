#pragma once
class TSP
{
private:
	double computingTime;

	//OpenCL data
	cl::Platform platform;
	cl::Device device;
	cl::Context context;

	// Travelling Salesman data
	#define NUMCITIES 13
	int cities[NUMCITIES];
	int coordinates[NUMCITIES*2];
	float results[NUMCITIES+2];

	//private functions
	void initData(void);
	void setArgsAndExecuteKernel(cl::Kernel kernel);

public:
	TSP(cl::Platform platform, cl::Device device, cl::Context context);
	TSP(void);
	~TSP(void);
	void executeTsp(void);
	double getComputingTime(void);
};

