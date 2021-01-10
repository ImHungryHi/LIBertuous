#include "Calculations.h"

Calculations::Calculations()
{
	this->calculationsPerSecond = 0;
	this->ResetCalculations();
	this->RestartTimer();
}

void Calculations::ResetCalculations()
{
	this->number = 0;
}

void Calculations::RestartTimer()
{
	this->calculationsPerSecond = 0;
	this->start = clock();
}

double Calculations::PeekTimer()
{
	this->final = clock() - this->start;

	return (double) this->final / (double)CLOCKS_PER_SEC;
}