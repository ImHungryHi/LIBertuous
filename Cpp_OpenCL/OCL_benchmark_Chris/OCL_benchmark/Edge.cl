//
// Copyright (c) 2009 Advanced Micro Devices, Inc. All rights reserved.
// VERSION 1.3 :
// Working structs
// Working struct-Array
// Check if something is already in solution
// Turn solution into full array of pieces
// Working with defines instead of constants
// Placing edgepieces on the edges and cornerpieces on the corners
// Rotating edgepieces so that the edges always have the 0 to the outside
// Checks if left-right and up-down fits. If not, try other piece until it fits.
// After 1000 tries or when there aren't any tiles left, break out of while.
// Changes:
// --------
// Fixed rotationfunction
// TODO:
// --------
// Rotate piece 


#define CORNER 4
#define EDGES 20
#define MID 25
#define LENGTH 49


/* Define a type point to be a tile with integer members up down left right */
typedef struct{
   short down;
   short left;
   short up;
   short right;
   short rot;
}tile;

void rotatePiece(tile *t)
{
	short temp = t->down;
	t->down = t->right;
	t->right = t->up;
	t->up = t->left;
	t->left = temp;
	t->rot++;
	t->rot %= 4;
}

__kernel void edge(__global short *pieces, __global short *output, __global short *solution){ 
	/*int const CORNER = 4;
	int const EDGES = 20;
	int const MID = 25;
	int const LENGTH = 49;*/
	size_t id = get_global_id(0);

	//Make structarray for all pieces
	tile tiles[LENGTH];
	for(int i = 0; i < LENGTH*4; i+=4) {
		tile t = {pieces[i+0],pieces[i+1],pieces[i+2],pieces[i+3],0};
		tiles[i/4] = t;
	}
	
	

	//Make Solution
	//short solution[LENGTH];
	for (int i = 0; i < LENGTH; ++i){
	  solution[i] = -1;
	}
	short solutionTiles = 0;
	short tilePointer = 0;
	short counter = 0;
	short tileOffset = 0;
	short unChecked = 1;
	short breakTest = 0;
	//As long as there isn't a full solution do this
	while(solutionTiles != LENGTH) {
		counter++;
		if(counter > 1000) {
			break;
		}
		//Check if tile is already in solution
		short edgeCorner = 0; //1 = Corner / 2 = Edge / 3 = Mid
		for(int i = 0; i < LENGTH; i++) {
			if(solutionTiles == 0 || solutionTiles == 6 || solutionTiles == 42 || solutionTiles == 48) {
				tilePointer = tileOffset;
				edgeCorner = 1;
				break;
			}
			else if(solutionTiles < 7 || solutionTiles > 41 || (solutionTiles%7) == 0 || (solutionTiles%7) == 6) {
				tilePointer = CORNER + tileOffset;
				edgeCorner = 2;
				break;
			}
			else {
				tilePointer = CORNER + EDGES + tileOffset;
				edgeCorner = 3;
				break;
			}
		}
		//Make the loop stop if no corner/edge/midpieces are left to test.
		if(edgeCorner == 1) {
			if(tilePointer >= CORNER) {
				breakTest = 1;
			}
		}
		if(edgeCorner == 2) {
			if(tilePointer >= (CORNER+EDGES)) {
				breakTest = 1;
			}
		}		
		if(edgeCorner == 3) {
			if(tilePointer >= (CORNER+EDGES+MID)) {
				breakTest = 1;
			}
		}
		if(breakTest ==1) {
			break;
		}
		while(unChecked == 1){
			unChecked = 0;
			for(int j = 0; j < LENGTH; j++) {
				if(solution[j] == tilePointer) {
					tilePointer++;
					unChecked = 1;
				}
			}
		}
		unChecked = 1;

		if(solutionTiles < 7) {
			while(tiles[tilePointer].up != 0) {
				rotatePiece(&tiles[tilePointer]);
			}
		} 
		if(solutionTiles > 41) {
			while(tiles[tilePointer].down != 0) {
				rotatePiece(&tiles[tilePointer]);
			}
		} 
		if((solutionTiles%7) == 0) {
			while(tiles[tilePointer].left != 0) {
				rotatePiece(&tiles[tilePointer]);
			}
		} 
		if((solutionTiles%7) == 6) {
			while(tiles[tilePointer].right != 0) {
				rotatePiece(&tiles[tilePointer]);
			}
		}
		if(solutionTiles == 0) {
			while(tiles[tilePointer].left != 0) {
				rotatePiece(&tiles[tilePointer]);
			}
			if (tiles[tilePointer].up != 0) rotatePiece(&tiles[tilePointer]);
		} 
		if(solutionTiles == 6) {
			while(tiles[tilePointer].up != 0) {
				rotatePiece(&tiles[tilePointer]);
			}
			if (tiles[tilePointer].right != 0) rotatePiece(&tiles[tilePointer]);
		}
		if(solutionTiles == 42) {
			while(tiles[tilePointer].down != 0) {
				rotatePiece(&tiles[tilePointer]);
			}
			if (tiles[tilePointer].left != 0) rotatePiece(&tiles[tilePointer]);
		}
		if(solutionTiles == 48) {
			while(tiles[tilePointer].right != 0) {
				rotatePiece(&tiles[tilePointer]);
			}
			if (tiles[tilePointer].down != 0) rotatePiece(&tiles[tilePointer]);
		}
		short continueTester = 0;
		//Rules for testing if the edges are correct
		if(solutionTiles!=0) {
			if(tiles[tilePointer].left != tiles[solution[solutionTiles-1]].right) {
				tileOffset++;
				continueTester = 1;
			}
		}
		if(solutionTiles > 6) {
			if(edgeCorner == 3 ) {
				if(tiles[tilePointer].up != tiles[solution[solutionTiles-7]].down) {
					for(int i = 0; i < 4; i++) {
						if(tiles[tilePointer].up == tiles[solution[solutionTiles-7]].down) {
							break;
						}
						rotatePiece(&tiles[tilePointer]);
						solution[48] = tilePointer;;
					}
				}
			}
			if(tiles[tilePointer].up != tiles[solution[solutionTiles-7]].down) {
				tileOffset++;
				continueTester = 1;
			}
		}
		if(continueTester == 1) {
			continue;
		}
		if(tilePointer == LENGTH) {
			break;
		}
		solution[solutionTiles] = tilePointer;
		solutionTiles++;
		tilePointer = 0;
		tileOffset = 0;
	}
	
	if (id<1) {
		for(int i = 0; i < LENGTH*4; i+=4) {
			short j = solution[i/4];
			if(j == -1) {
				output[i] = -1;
				output[i+1] = -1;
				output[i+2] = -1;
				output[i+3] = -1;
				solution[i/4+LENGTH] = -1; 
			} else {
				output[i] = tiles[j].down;
				output[i+1] = tiles[j].left;
				output[i+2] = tiles[j].up;
				output[i+3] = tiles[j].right;
				solution[i/4+LENGTH] = tiles[j].rot; 
			}
		}
	}

	/*
	if (id<LENGTH) {
		//output[id] = tiles[id].rot;
	} else {
		//output[id] = 5;
	}*/
}