#include <time.h>

class Calculations
{
	public:
		// Fields
		int number;
		double calculationsPerSecond;
		double averagePerSecond;
		clock_t start;
		clock_t final;

		// Methods
		Calculations();
		void RestartTimer();
		double PeekTimer();
		void ResetCalculations();
};