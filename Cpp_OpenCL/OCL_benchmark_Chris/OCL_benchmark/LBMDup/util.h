#pragma once
class util
{
	public:
		static cl::Platform GetPlatform(void);
		static std::vector<cl::Platform> GetPlatforms();
		static cl::Device GetDevice(cl::Platform platform);
		static std::vector<cl::Device> GetDevices(cl::Platform platform);
		static cl::Context CreateContext(cl_device_type deviceType);
		static cl::Kernel LoadKernelFromFile(cl::Context context, std::string path, char *methodName);
		static int CheckError(cl_int error, std::string s);
};