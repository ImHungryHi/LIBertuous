#include "LatticeBoltzmann.h"
#include "LatticeSite.h"
#include "ColorScale.h"
#include "stdafx.h"

#include <stdlib.h>
#include <iostream>

using namespace std;

LatticeBoltzmann::LatticeBoltzmann(const int d[2])
{
	dims[0] = d[0];
	dims[1] = d[1];

	sites = new LatticeSite*[dims[0]];
	sites_ = new LatticeSite*[dims[0]];

	for (int i=0; i<dims[0]; i++)
	{
		sites[i] = new LatticeSite[dims[1]];
		sites_[i] = new LatticeSite[dims[1]];
	}

	// allocate memory for rho
	rho = new double*[dims[0]];
	for (int x=0; x<dims[0]; x++)
		rho[x] = new double[dims[1]];

	// allocate memory for velocity
	u = new double**[dims[0]];
	for (int x=0; x<dims[0]; x++)
	{
		u[x] = new double*[dims[1]];
		
		for (int y=0; y<dims[1]; y++)
			u[x][y] = new double[2];
	}

	reset();
}

void LatticeBoltzmann::reset()
{
	this->calculations = Calculations();
	this->motion = false;
	double u[2] = {0, 0};

	for (int y=0; y<dims[1]; y++)
	{
		for (int x=0; x<dims[0]; x++)
		{
			if (x == 0 || x == dims[0]-1 || y == 0 || y == dims[1]-1)
			{
				sites[x][y].init(LatticeSite::Boundary, 10.0, u);
				sites_[x][y].init(LatticeSite::Boundary, 10.0, u);
			}
			else
			{
				sites[x][y].init(LatticeSite::Fluid, 10.0, u);
				sites_[x][y].init(LatticeSite::Fluid, 10.0, u);
			}
		}
	}
}

LatticeBoltzmann::~LatticeBoltzmann()
{
	delete sites;
	delete sites_;
}


void LatticeBoltzmann::streamToNeighbors(int x, int y)
{
	for (int k=0; k<9; k++)
	{
		int nx = x + LatticeSite::e[k][0];
		int ny = y + LatticeSite::e[k][1];

		if (nx >= 0 && nx < dims[0] && ny >= 0 && ny < dims[1])
			sites_[nx][ny].f[k] = sites[x][y].f[k];
	}
}

void LatticeBoltzmann::update()
{
	LatticeSite **swap;

	#pragma omp parallel for
	for (int y=0; y<dims[1]; y++)
	{
		for (int x=0; x<dims[0]; x++)
		{
			// do collision and streaming in one loop
			sites[x][y].collide(rho[x][y], u[x][y]);
			streamToNeighbors(x, y);
			

		}
	}

	swap = sites;
	sites = sites_;
	sites_ = swap;
}

void LatticeBoltzmann::setSite(int x, int y, LatticeSite::SiteType type, double u[2])
{
	sites[x][y].setType(type);
	sites_[x][y].setType(type);

	sites[x][y].setU(u);
	sites_[x][y].setU(u);

	//cout << "x:" << x << " y:" << y << " - type:" << type << " - fluid velocity:" << u[0] << " by " << u[1] << "\n";
}

void LatticeBoltzmann::getDensityAndVelocityField(double **&rp, double ***&up)
{
	rp = rho;
	up = u;
}

LatticeSite LatticeBoltzmann::getSite(int x, int y)
{
	if (this->motion)
	{
		calculations.number++;
		double peek = calculations.PeekTimer() * 1000;

		if (((int)peek % 1000) == 0)
		{
			calculations.calculationsPerSecond = calculations.number / peek;
			calculations.averagePerSecond = (calculations.averagePerSecond + calculations.calculationsPerSecond) / 2;

			cout << calculations.calculationsPerSecond << " kCalcs/s\n";
		}
	}

	return sites[x][y];
}

void LatticeBoltzmann::resetCalculationCounter()
{
	calculations.averagePerSecond = 0;
	calculations.ResetCalculations();
	calculations.RestartTimer();
}

void LatticeBoltzmann::setMotionOn()
{
	if (!this->motion)
	{
		this->calculations.RestartTimer();
	}

	this->motion = true;
}

void LatticeBoltzmann::setMotionOff()
{
	this->motion = false;
	this->calculations.ResetCalculations();
}

bool LatticeBoltzmann::getMotionStatus()
{
	return this->motion;
}

double LatticeBoltzmann::GetAverageCalculationsPerSecond()
{
	return calculations.averagePerSecond;
}