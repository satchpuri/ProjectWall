using namespace std;
#include <iostream>
#include "Heap.h"
#include <fstream>

//update
#define update(i, j, maxDistance, data, heap)	\
if (maxDistance[i] + data[j] < maxDistance[j])	\
{												\
	maxDistance[j] = maxDistance[i] + data[j];	\
	trace[j] = i;								\
	heap.push(maxDistance[j] + h[j], j);		\
}

void aStar()
{
	//initialize visited as false for each tile
	bool visited[25] = {};
	//g(n)
	unsigned int maxDistance[25];
	int trace[25];
	int goal;
	//5x5 matrix
	int data[25];

	for (int i = 0; i < 25; i++)
	{
		maxDistance[i] = -1;
	}
	//reading map
	std::fstream file("map.txt", std::ios_base::in);
	for (int i = 0; i < 5; i++)
	{
		for (int j = 0; j < 5; j++)
		{
			file >> data[(i * 5 + j)];

			if (0 == data[i * 5 + j] && i * 5 + j != 12)
			{
				goal = i * 5 + j;
			}
		}
	}
	file.close();

	//estimated value
	int h[25];
	for (int i = 0; i < 25; i++)
	{
		int goalColNum = goal % 5;
		int goalRowNum = goal / 5;

		//current col
		int currentColNum = i % 5;
		int currentRowNum = i/ 5;
		
		h[i] = labs(currentColNum - goalColNum) + labs(currentRowNum - goalRowNum);

	}
	
	//make a heap for the map
	Heap<int, int> heap = Heap<int, int>();
	heap.push(maxDistance[12], 12);

	while (heap.size > 0)
	{
		int n = heap.pop();
		//is visted
		if (visited[n])
		{
			continue;
		}
		visited[n] = true;

		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				//to print the map
				char render = (char)((data[i * 5 + j]) + (int)'0');

				if (visited[i * 5 + j])
				{
					render = 'x';
				}
				cout << render << ' ';
			}
			cout << endl;
		}
		cin.get();

		
		trace[12] = -1;
		//checking for neibhors
		if (n == goal)
		{
			cout << "Distance: " << maxDistance[n]+1 << endl;
			cout << "Path: ";
			while (trace[n] != -1)
			{
				cout << trace[n] << ' ';
				n = trace[n];
			}
			cin.get();
			return;
		}
		//up 
		if (n / 5 > 0)
		{
			update(n, n - 5, maxDistance, data, heap);
		}
		//Down 
		if (n / 5 < 4)
		{
			update(n, n + 5, maxDistance, data, heap);
		}
		//Left
		if (n % 5 > 0)
		{
			update(n, n - 1, maxDistance, data, heap);
		}
		if (n % 5 < 4)
		{
			update(n, n + 1, maxDistance, data, heap);
		}	
		
	}
}
int main()
{
	//calling astar
	aStar();


	////test min-heap
	//Heap<float, char> heap = Heap<float, char>();
	//heap.push(2.8f, 'a');
	//heap.push(2.5f, 'z');
	//heap.push(5.5f, 's');
	//heap.push(6.3f, 'b');

	//while (heap.size > 0)
	//{
	//	char temp = heap.pop();
	//	cout << temp << endl;
	//}
	cin.get();
	return 0;
}

