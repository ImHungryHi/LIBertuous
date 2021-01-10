//*
//brute force tsp kernel, by Geoffrey Van Landeghem
//*

#define NUMCITIES		13
#define PATHLENGTH		14	//13 cities to visit + 1 extra field for the return path
#define NUM_PERM_CITIES	10	//first city always the same and second/third city ~ threadId, so we need to perumate 3 cities less


// NUMCITIES	= number of cities used in the solution
// *cities		= one dimensional array of cities ID's
// *coordinates = two dimensional array transformed to a one dimenional array, contains the city coordinates
// *results		= a resulting array, this will pass back a solution to the host
//					=> last member of array = the travelled distance
//					=> preceding members   = path
__kernel void tsp( __global int *cities,
				   __global int *coordinates, 
				   __global float *results )
{ 
	int path[PATHLENGTH];					// path exists off all city to visit including the return node (= start city)
	float totalDistance = 0;				//the total distance we travelled for a path

	int secondCity = get_global_id(0);
	int thirdCity = get_global_id(1);

	if (secondCity == thirdCity) return;	//same thread id's ~> skip calculation if we visit the same city twice



	int citiesToVisit[NUM_PERM_CITIES+1]; // -> +1 because we're going to reuse the array later on for the permutations
	/*********** DETERMINE CITIES TO VISIT *********/
	for (int j = 0; j < NUM_PERM_CITIES; j++) { //loop through citiesToVisit array and fill

		for (int k = 0; k < NUMCITIES-1; k++) { //loop through cities array, (-1 because you do not need to check the start city which is at the highest index)
			if(cities[k] == secondCity) continue;	//if same as secondCity, do not add because we'll explicitly add it ourselves
			if(cities[k] == thirdCity) continue;	//if same as thirdCity, do not add because we'll explicitly add it ourselves

			//if citiesToVisit is empty, add first item we find that is different from thread id
			if(j == 0) {
				citiesToVisit[j] = cities[k];
				break;
			}
			//else if citiesToVisit not empty...
			else {						
				// loop through each node we've added and not add it again
				bool added = true;
				for (int l = 0; l < j; l++) {
					if(citiesToVisit[l] == cities[k]) {
						added = false;
						break; //already added, try another city in cities array
					}	
				}

				if(!added) continue; 
				// no equal city found in loop... so this city has not been added yet, add it!
				citiesToVisit[j] = cities[k];
				break;
			}

		}
	}




	/*********** GENERATE PATH ******************/
	path[0] = cities[NUMCITIES-1];	//start city
	path[1] = cities[secondCity];	//second city ~> thread id in dimension 1
	path[2] = cities[thirdCity];	//third city ~> thread id in dimension 2
	//4rd until last city
	for (int u = 3; u < PATHLENGTH; u++) path[u] = citiesToVisit[u-3]; 
	path[NUMCITIES] = path[0]; //return to first city




	/*********** GENERATE PERMUTATION FOR PATH ******************/
	// NOTICE:  Copyright 1991-2008, Phillip Paul Fuchs
	//http://www.oocities.org/permute_it/01example.html
	for (int n = 0; n < NUM_PERM_CITIES; n++) citiesToVisit[n] = n;
    citiesToVisit[NUM_PERM_CITIES] = NUM_PERM_CITIES; // citiesToVisit[NUM_PERM_CITIES] > 0 controls iteration and the index boundary for i
	int i = 1;   // setup first swap points to be 1 and 0 respectively (i & j)
	int j = 1;

	//generate permutation
    while(i < NUM_PERM_CITIES) { //while i < 10

        citiesToVisit[i]--;             // decrease index "weight" for i by one
		j = (i % 2) * citiesToVisit[i]; // IF i is odd then j = p[i] otherwise j = 0

		int tmp = path[j+3];			// swap(path[j], path[i])
		path[j+3] = path[i+3];
		path[i+3] = tmp;

	
		
		/*********** CALCULATE DISTANCE ******************/
		totalDistance = 0; //reset
		int Ax, Bx, Ay, By;
		for (int n = 0; n < NUMCITIES; n++) {
			//get positive deltaX
			Ax = coordinates[path[n]];
			Bx = coordinates[path[n+1]];
			int deltaX = Ax > Bx ? Ax - Bx : Bx - Ax;

			//get positive deltaY (y values have offset of NUMCITIES in memory)
			Ay = coordinates[NUMCITIES + path[n]];
			By = coordinates[NUMCITIES + path[n+1]];
			int deltaY = Ay > By ? Ay - By : By - Ay;

			if (deltaX == 0) totalDistance += deltaY;
			else if (deltaY == 0) totalDistance += deltaX;
			else totalDistance += sqrt( (float)(deltaX*deltaX + deltaY*deltaY) );
		}



		/**************** SAVE RESULTS ******************/
		//the calculated route is the newest shortest route, so save it
		if ( totalDistance < results[PATHLENGTH] ) {
			//copy shortest distance to results array
			results[PATHLENGTH] = totalDistance;
			

			//copy path to results array
			for (int n = 0; n < PATHLENGTH; n++) results[n] = path[n];
		}



		/**************** PERMUTATION RESET ******************/
		i = 1;						// reset index i to 1 (assumed)
		// while (p[i] == 0)
		while (!citiesToVisit[i]) {
			citiesToVisit[i] = i;   // reset p[i] zero value
			i++;					// set new index value for i (increase by one)
		}

		
    }//end while to generate permutation

}