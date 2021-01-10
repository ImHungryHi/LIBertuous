#ifndef LATTICE_BOLTZMANN_H
#define LATTICE_BOLTZMANN_H

#include "LatticeSite.h"
#include "ColorScale.h"
#include "Calculations.h"

class LatticeBoltzmann
{
	private:
		int dims[2];

		double **rho;
		double ***u;
		bool motion;

		LatticeSite **sites;
		LatticeSite **sites_;

		void streamToNeighbors(int x, int y);
		Calculations calculations;

	public:
		LatticeBoltzmann(const int d[2]);
		~LatticeBoltzmann();

		void reset();

		void setMotionOn();
		void setMotionOff();
		bool getMotionStatus();
		LatticeSite getSite(int x, int y);
		void setSite(int x, int y, LatticeSite::SiteType type, double u[2]);
		void resetCalculationCounter();
		void getDensityAndVelocityField(double **&rho, double ***&u);
		void stream();
		void update();
		double LatticeBoltzmann::GetAverageCalculationsPerSecond()
};

#endif

