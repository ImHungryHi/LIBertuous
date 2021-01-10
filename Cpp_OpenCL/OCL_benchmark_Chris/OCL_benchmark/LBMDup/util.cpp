#pragma once
#define CL_USE_DEPRECATED_OPENCL_1_1_APIS

#include <iostream>
#include <fstream>
#include <vector>
#include <sdkddkver.h>
#include <CL\cl.hpp>

using std::cout;
using std::cin;
using cl::Platform;
using cl::Device;
using cl::Context;
using cl::Kernel;
using cl::Program;

#include "util.h"

cl::Platform util::GetPlatform(void)
{
	std::vector<Platform> platforms = GetPlatforms();
	int sz = platforms.size();

	if (sz == 1) return platforms[0];
	else
	{
		
		std::string choice = "";

		cout << "Found " << sz << " platforms, choose a platform [1-" << sz << "]:\n";
		std::getline(cin, choice);

		for (int x = 0; x < sz; x++)
		{
			if (choice.compare("" + (x + 1)) == 0) return platforms[x];
		}
	}
	return NULL;
}

std::vector<cl::Platform> util::GetPlatforms(void)
{
	cl_int error = CL_SUCCESS;

	std::vector<Platform> platforms;
	error = Platform::get(&platforms);

	CheckError(error, "Failed to retrieve platforms");

	return platforms;
}

cl::Device util::GetDevice(Platform platform)
{
	std::vector<Device> devices = GetDevices(platform);
	int sz = devices.size();
	if (sz == 1) return devices[0];
	else
	{
		for (int x = 0; x < sz; x++)
		{
			if (devices[x].getInfo<CL_DEVICE_TYPE>() == CL_DEVICE_TYPE_GPU)
			{
				cout << "Selecting GPU device " << (x + 1) << "\n";
				return devices[x];
			}
		}

		cout << "Unable to find GPU devices, selecting the first CPU device\n";
		return devices[0];
	}
}

std::vector<cl::Device> util::GetDevices(Platform platform)
{
	cl_int error = CL_SUCCESS;
	std::vector<Device> devices;

	error = platform.getDevices(CL_DEVICE_TYPE_ALL, &devices);
	CheckError(error, "Failed to retrieve devices");

	return devices;
}

cl::Context util::CreateContext(cl_device_type deviceType)
{
	cl_int error = CL_SUCCESS;

	cl_platform_id platform = NULL;
	clGetPlatformIDs(1, &platform, NULL);
	cl_context_properties cps[3] = { CL_CONTEXT_PLATFORM, (cl_context_properties)platform, 0};
	/*cl_device_id devices[256];
	clGetDeviceIDs(platform, deviceType, 256, devices, NULL);
	cl_device_id device = devices[0];
	cl_context context = clCreateContext(cps, 1, &device, NULL, NULL, &error);*/

	Context context(deviceType, cps, NULL, NULL, &error);
	CheckError(error, "Failed to create context");
	return context;
}

cl::Kernel util::LoadKernelFromFile(cl::Context context, std::string path, char *methodName)
{
	cl_int error = CL_SUCCESS;

	// enumerating context devices
	std::vector<Device> devices = context.getInfo<CL_CONTEXT_DEVICES>(&error);
	CheckError(error, "Failed to retrieve context devices info");

	cout << "Loading and compiling CL source file\n";

	// load kernel file from disk
	std::ifstream file(path);
	if (!file.is_open()) CheckError(1, "Failed reading " + path);

	std::string progSource(std::istreambuf_iterator<char>(file), (std::istreambuf_iterator<char>()));

	// make a source object
	Program::Sources sources(1, std::make_pair(progSource.c_str(), progSource.length()));

	//creates an OpenCL program object for a context, and loads the source code specified by the text strings in each 
	//element of the vector sources into the program object.
	Program program = Program(context, sources, &error);
	CheckError(error, "Failed opening the OpenCL program");

	//builds (compilers and links) a program executable from the program source or binary for all the devices or a 
	//specific device(s) in the OpenCL context associated with program.
	error = program.build(devices);
	CheckError(error, "Failed building the OpenCL program");
	Kernel kernel(program, methodName, &error);
	CheckError(error, "Failed building the kernel from the OpenCL program");

	return kernel;
}

int util::CheckError(cl_int error, std::string s)
{
	if (error != CL_SUCCESS)
	{
		cout << s.c_str() << "\nPress enter to exit\n";
		
		char key[1];
		cin.getline(key, 1);

		exit(EXIT_FAILURE);
	}

	return CL_SUCCESS;
}