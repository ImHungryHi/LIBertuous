// util class
// class with handy helper functions withc makes life just that tad easier during OpenCL hacking
// by ICT @ kahosl belgium


#pragma once


class util
{
public:
	static void waitForReturnKey(void);
	static int check_error(cl_int err, std::string s);
	static std::string askInput(std::string question);
	static double showComputingTime(clock_t clock1,clock_t clock2);

	static void showPlatforms(void);
	static std::vector<cl::Platform> getPlatforms(void);
	static void showDevices(cl::Platform platform);
	static std::vector<cl::Device> getDevices(cl::Platform platform);
	static cl::Context createContext(cl_device_type deviceType);
	static cl::Kernel loadAndBuildKernelFromFile (cl::Context context, std::string path, char *methodName);
	static cl::Kernel loadAndBuildKernelFromString (cl::Context context, std::string sourceCode, char *methodName);
	
	static cl::Platform GetPlatform(void);
	static std::vector<cl::Platform> GetPlatforms();
	static cl::Device GetDevice(cl::Platform platform);
	static std::vector<cl::Device> GetDevices(cl::Platform platform);
	static cl::Context CreateContext(cl_device_type deviceType);
	static cl::Kernel LoadKernelFromFile(cl::Context context, std::string path, char *methodName);
	static int CheckError(cl_int error, std::string s);
};