// OpenCL kernel for Lattice Boltzmann computations by Chris De Smedt

__kernel void LBM()
{
	// Thread ID
	size_t i = get_global_id(0);
}