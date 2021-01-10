#include "StdAfx.h"
#include "util.h"


#pragma region Helper functions
//simply waits until return is pressed
void util::waitForReturnKey(void) {
	char k[1];
	cin.getline(k, 1);
}


//checks if a CL method throws an error code or not.
int util::check_error(cl_int err, std::string s) {
	if(err != CL_SUCCESS)
	{
		cout << s.c_str() << " -> error(" << err << ")" << endl; //std::endl = '\n', but std::endl flushes output stream where '\n' does not
		cout << "Press RETURN button to EXIT..." << endl;
		waitForReturnKey();
		//end program
		exit(EXIT_FAILURE);
	}
	return CL_SUCCESS;
}


// asks a question and return the answer as a string (in upper case!)
std::string util::askInput(std::string question) {
	cout << question;
	std::string inputString = "";
	std::getline(cin, inputString);
	for (int i = 0; i < (int)inputString.size(); i++) inputString[i] = toupper(inputString[i]);
	return inputString;
}


//shows the calculation time, returns the difference as double in ms
double util::showComputingTime(clock_t clock1,clock_t clock2) {
	double diffticks=clock1-clock2;
	double diffms=(diffticks*1000)/CLOCKS_PER_SEC;
	int seconds = diffms/1000;
	int ms = diffms - (seconds * 1000);
	cout << "Computing time: " << seconds << "s "<< ms << "ms" << endl << endl;
	return diffms;
}
#pragma endregion







// show the info of all available platforms
void util::showPlatforms(void) {
	cl_int err = CL_SUCCESS;  //holds error values

	std::vector<Platform> platforms;
	err = Platform::get(&platforms);

	check_error(err, "Platform::get() failed"); 	//if not succeeded, print error and exit

	//make an iterator which loops through all platforms
	std::vector<Platform>::iterator i;

	if(platforms.size() > 0)
	{
		for(i = platforms.begin(); i != platforms.end(); ++i)
		{
			cout << "OpenCL \tPlatform: "	<< "\t" << (*i).getInfo<CL_PLATFORM_NAME>(&err).c_str()			<< endl;
			cout << "\tVendor: "			<< "\t" << (*i).getInfo<CL_PLATFORM_VENDOR>(&err).c_str()		<< endl;
			cout << "\tVersion: "			<< "\t" << (*i).getInfo<CL_PLATFORM_VERSION>(&err).c_str()		<< endl;
		}
	}
	else check_error(1, "platforms.size() not bigger than 0 => no platforms found"); 	//if not succeeded, print error and exit

	//check if we successfully retrieved info, otherwise print error and exit
	check_error(err, "Platform::getInfo() failed"); 	//if not succeeded, print error and exit
}




//gets all available platforms
std::vector<cl::Platform> util::getPlatforms(void) {
	cl_int err = CL_SUCCESS;  //holds error values

	std::vector<Platform> platforms;
	err = Platform::get(&platforms);

	check_error(err, "Platform::get() failed"); 	//if not succeeded, print error and exit

	return platforms;
}




//show all available platform devices
void util::showDevices(cl::Platform platform) {
	cl_int err = CL_SUCCESS;  //holds error values

	//make an array (=vector) of devices
	std::vector<cl::Device> devices;
	err = platform.getDevices(CL_DEVICE_TYPE_ALL, &devices);

	check_error(err, "Platform::getDevices() failed"); 	//if not succeeded, print error and exit

	//and loop through each object
	cout << devices.size() << " device(s) found" << endl;
	if(devices.size() > 0)
	{
		for(int i = 0; i < (int)devices.size(); ++i)
		{
			cout << "*******************************" << endl;
			cout << "  Device:"				<< "\t\t" << devices[i].getInfo<CL_DEVICE_NAME>(&err).c_str()	<< endl;
			cout << "  Vendor:"				<< "\t\t" << devices[i].getInfo<CL_DEVICE_VENDOR>(&err).c_str()	<< endl;
			cout << "  Version:"			<< "\t\t" << devices[i].getInfo<CL_DEVICE_VERSION>(&err).c_str()	<< endl;
			cout << "  Driver version:"		<< "\t" << devices[i].getInfo<CL_DRIVER_VERSION>(&err).c_str()	<< endl;
			cl_device_type type = devices[i].getInfo<CL_DEVICE_TYPE>(&err);
			if (type == CL_DEVICE_TYPE_CPU)
				cout << "  Device type:"	<< "\t\t" << "CL_DEVICE_TYPE_CPU"	<< endl;
			if (type == CL_DEVICE_TYPE_GPU)
				cout << "  Device type:"	<< "\t\t" << "CL_DEVICE_TYPE_GPU"	<< endl;
			cout << "  Compute units:"		<< "\t" << devices[i].getInfo<CL_DEVICE_MAX_COMPUTE_UNITS>(&err) << endl;
			cout << "  Work item dimensions:"	<< "\t" << devices[i].getInfo<CL_DEVICE_MAX_WORK_ITEM_DIMENSIONS>(&err) << endl;
			cout << "  Work item size dim0:"	<< "\t" << devices[i].getInfo<CL_DEVICE_MAX_WORK_ITEM_SIZES>(&err)[0] << endl;
			cout << "  Work item size dim1:"	<< "\t" << devices[i].getInfo<CL_DEVICE_MAX_WORK_ITEM_SIZES>(&err)[1] << endl;
			cout << "  Work item size dim2:"	<< "\t" << devices[i].getInfo<CL_DEVICE_MAX_WORK_ITEM_SIZES>(&err)[2] << endl;
			cout << "  Work group size:"	<< "\t" << devices[i].getInfo<CL_DEVICE_MAX_WORK_GROUP_SIZE>(&err) << endl;
			cout << "  Clock frequency:"	<< "\t" << devices[i].getInfo<CL_DEVICE_MAX_CLOCK_FREQUENCY>(&err) << " MHz" << endl;
			cout << "  Address bits:"		<< "\t\t" << devices[i].getInfo<CL_DEVICE_ADDRESS_BITS>(&err) << endl;
		}
	}
	else check_error(1, "devices.size() not bigger than 0 => no devices found for this platform"); 	//if not succeeded, print error and exit

	//check if we successfully retrieved info, otherwise print error and exit
	check_error(err, "Device::getInfo() failed"); 	//if not succeeded, print error and exit
}




// returns an array of devices
std::vector<cl::Device> util::getDevices(cl::Platform platform) {
	cl_int err = CL_SUCCESS;  //holds error values

	//make an array (=vector) of devices
	std::vector<cl::Device> devices;
	err = platform.getDevices(CL_DEVICE_TYPE_ALL, &devices);

	check_error(err, "Platform::getDevices() failed"); 	//if not succeeded, print error and exit

	return devices;
}



//returns a context
cl::Context util::createContext(cl_device_type deviceType) {
	cl_int err = CL_SUCCESS;  //holds error values

	if (deviceType == CL_DEVICE_TYPE_CPU)
		cout << "# Creating a CPU context" << endl;
	if (deviceType == CL_DEVICE_TYPE_GPU)
		cout << "# Creating a GPU context" << endl;

	cout << endl;
	Context context(deviceType, NULL, NULL, NULL, &err);
	check_error(err, "Context::Context() failed"); 	//if not succeeded, print error and exit
	return context;
}



//load and builds a .cl kernel from a file on disk
cl::Kernel util::loadAndBuildKernelFromFile (cl::Context context, std::string path, char *methodName) {
	cl_int err = CL_SUCCESS;  //holds error values

	cout << "# Enumerating context devices" << endl;
	std::vector<Device> devices = context.getInfo<CL_CONTEXT_DEVICES>(&err);
	check_error(err, "Context::getInfo<CL_CONTEXT_DEVICES>() failed"); 	//if not succeeded, print error and exit

	cout << "# Loading and compiling CL source file" << endl;

	//load kernel file from disk
	std::ifstream file(path);
	if (!file.is_open()) 
		check_error(1, "reading " + path + " failed, is it in the correct directory?"); 	//if not succeeded, print error and exit

	std::string progSource(std::istreambuf_iterator<char>(file), (std::istreambuf_iterator<char>()));

	//make a source object
	Program::Sources sources(1, std::make_pair(progSource.c_str(), progSource.length()));

	//creates an OpenCL program object for a context, and loads the source code specified by the text strings in each 
	//element of the vector sources into the program object.
	Program program = Program(context, sources, &err);
	check_error(err, "Program::Program() failed"); 	//if not succeeded, print error and exit

	//builds (compilers and links) a program executable from the program source or binary for all the devices or a 
	//specific device(s) in the OpenCL context associated with program.
	err = program.build(devices);
	check_error(err, "Program::build() failed"); 	//if not succeeded, print error and exit
	//make a kernel from the compiled program
	//name (2nd argument) is a function name in the program declared with the __kernel qualifier.
	Kernel kernel(program, methodName, &err);
	check_error(err, "Kernel::Kernel() failed"); 	//if not succeeded, print error and exit

	return kernel;
}



//load and builds a .cl kernel from string
cl::Kernel util::loadAndBuildKernelFromString (cl::Context context, std::string sourceCode, char *methodName) {
	cl_int err = CL_SUCCESS;  //holds error values

	cout << "# Enumerating context devices" << endl;
	std::vector<Device> devices = context.getInfo<CL_CONTEXT_DEVICES>(&err);
	check_error(err, "Context::getInfo<CL_CONTEXT_DEVICES>() failed"); 	//if not succeeded, print error and exit

	cout << "# Loading and compiling CL source file" << endl;

	//make a source object
	Program::Sources sources(1, std::make_pair(sourceCode.c_str(), sourceCode.length()));

	//creates an OpenCL program object for a context, and loads the source code specified by the text strings in each 
	//element of the vector sources into the program object.
	Program program = Program(context, sources, &err);
	check_error(err, "Program::Program() failed"); 	//if not succeeded, print error and exit

	//builds (compilers and links) a program executable from the program source or binary for all the devices or a 
	//specific device(s) in the OpenCL context associated with program.
	err = program.build(devices);
	check_error(err, "Program::build() failed"); 	//if not succeeded, print error and exit

	//make a kernel from the compiled program
	//name (2nd argument) is a function name in the program declared with the __kernel qualifier.
	Kernel kernel(program, methodName, &err);
	check_error(err, "Kernel::Kernel() failed"); 	//if not succeeded, print error and exit

	return kernel;

}

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