// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

#define CL_USE_DEPRECATED_OPENCL_1_1_APIS	//needed when openCL 1.2 headers don't work
#define __NO_STD_STRING						//Use cl::vector instead of STL version

#include "targetver.h"

#include <stdio.h>
#include <tchar.h>

#include <cstdio>
#include <cstdlib>
#include <iostream>
#include <string>
#include <fstream>
#include <iterator>
#include <time.h>

#include <CL/cl.hpp> //cpp api, for c api use CL/cl.h

#include "util.h"
#include "TSP.h"
#include "EdgeMatching.h"
#include "LBMMain.h"

using std::cout; using std::endl; using std::cin;
using cl::Platform; using cl::Device; using cl::Context; using cl::Kernel; using cl::Program; using cl::CommandQueue; using cl::Buffer;


