#pragma once
class LBMMain
{
	public:
		void ExecuteLBM();
		void draw_densities(int x, int y);
		void draw_velocities();
		void draw_particles();
		void display();
		void update();
		void mouse(int button, int state, int x, int y);
		void motion(int x, int y);
		void reshape(int w, int h);
		void keyboard(unsigned char key, int x, int y);
		void cleanup();
		double GetAverageCalculationsPerSecond();
};