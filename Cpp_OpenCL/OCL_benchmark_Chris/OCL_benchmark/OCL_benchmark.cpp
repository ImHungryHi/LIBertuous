// OCL_benchmark.cpp : Defines the entry point for the console application.
// OpenCL benchmarking program
// includes Travelling Salesman Problem, Edgde Matching Puzzle and Water Simulation (Lattice Boltzmann)

#include "stdafx.h"

// the total computing time fields
double computingTimeTSP = 0;
double computingTimeEM = 0;
double computingTime = 0;


#pragma region OpenCL_Initialization
// shows the user all available platforms and lets him select one,
// auto selects first platform if only one available,
// exits if none available
Platform selectPlatform() {
	//show platforms
	util::showPlatforms();

	//get platforms as array and ask user which one to use
	std::vector<Platform> platforms = util::getPlatforms();

	//select first platform if any is found
	if (platforms.size() == 1) {
		cout << "# found one platform, auto selecting it" << endl;
		cout << endl;
		return platforms[0];
	}
	//if more options, let the user select
	else {
		cout << "# found " << platforms.size() << " platforms";
		std::string choosenplatform = util::askInput(", please choose one by giving its number: ");
		cout << endl;
		if(choosenplatform.compare("1") == 0) return platforms[0];
		if(choosenplatform.compare("2") == 0) return platforms[1];
		if(platforms.size() > 2 && choosenplatform.compare("3") == 0) return platforms[2];
	}

	cout << "# no platform found or input incorrect" << endl;
	exit(EXIT_FAILURE); //exit if nothing got selected
}



// shows the user all available devices and auto-selects one (prefers GPU),
// exits if none available
Device selectDevice(Platform platform) {
	util::showDevices(platform);
	std::vector<cl::Device> devices = util::getDevices(platform);
	if (devices.size() == 1) {
		cout << "# found one device, auto selecting it" << endl;
		cout << endl;
		return devices[0];
	}
	else {
		std::string choosendevice = util::askInput("# what type of device would you like to select? (type 'CPU' or 'GPU'): ");
		cout << choosendevice <<endl;
		//loop over all devices and try selecting a GPU
		for (int i = 0; i < (int) devices.size(); i++) {

			cl_device_type type = devices[i].getInfo<CL_DEVICE_TYPE>();

			if (choosendevice.compare("GPU") == 0 && type == CL_DEVICE_TYPE_GPU) {
				cout << "# found a GPU device, selecting it" << endl;
				cout << endl;
				return devices[i];
			}
			if (choosendevice.compare("CPU") == 0 && type == CL_DEVICE_TYPE_CPU) {
				cout << "# found a CPU device, selecting it" << endl;
				cout << endl;
				return devices[i];
			}
		}

		//if no GPU is found, auto-select first device
		cout << "# did not find a suitable device, try auto-selecting first device" << endl;
		cout << endl;
		return devices[0];
	}
}

#pragma endregion




//main program
int main(int argc, char *argv[]) {
	cout << "-----------------------------" << endl;
	cout << "ICT @ kahosl OpenCl benchmark" << endl;
	cout << "-----------------------------" << endl;

	//select platform
	Platform platform = selectPlatform();

	//select device
	Device device = selectDevice(platform);

	//create context
	Context context = util::createContext(device.getInfo<CL_DEVICE_TYPE>());

	//Execute Travelling Salesman
	TSP tsp = TSP(platform, device, context);
	tsp.executeTsp();
	computingTime += tsp.getComputingTime();
	computingTimeTSP = tsp.getComputingTime();
	
	//Execute Edgematching
	EdgeMatching em = EdgeMatching(platform, device, context);
	em.executeEdgeMatching();
	computingTimeEM += em.getComputingTimeEM();

	//Execute Lattice Boltzmann
	LBMMain lbm = LBMMain();
	lbm.ExecuteLBM();
	lbm.GetAverageCalculationsPerSecond();
	
	//show TSP time
	int seconds = computingTimeTSP/1000;
	int ms = computingTimeTSP - (seconds*1000);
	cout << "-----------------------------" << endl;
	cout << "Traveling salesman problem computing time: " << seconds << "s "<< ms << "ms" << endl;
	cout << "-----------------------------" << endl;

	//show EM time
	seconds = computingTime/1000;
	ms = computingTime - (seconds*1000);
	cout << "-----------------------------" << endl;
	cout << "Edgematching computing time: " << seconds << "s "<< ms << "ms" << endl;
	cout << "-----------------------------" << endl;
	
	//show TOTAL time
	seconds = computingTime/1000;
	ms = computingTime - (seconds*1000);
	cout << "-----------------------------" << endl;
	cout << "Total computing time: " << seconds << "s "<< ms << "ms" << endl;
	cout << "-----------------------------" << endl;

	cout << "Press RETURN button to EXIT..." << endl;

	util::waitForReturnKey();
	return 0;
}

