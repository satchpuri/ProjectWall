#pragma once
template <typename T, typename U>
class Heap
{
public:
	static int const MAX_SIZE = 100;
	int size = 0;
	T key[MAX_SIZE];
	U val[MAX_SIZE];

	//insert
	void push(T newKey, U newVal);
	//delete
	U pop();

	//constructor
	Heap();
	//destructor
	~Heap();

private:
	void heapifyUp(int i);
	void heapifyDown(int i);

	int leftChild(int i);
	int rightChild(int i);
	int parent(int i);
	void swap(int i, int j);
};

template<typename T, typename U>
void Heap<T, U>::push(T newKey, U newVal)
{
	//if heap size < max size
	if (size < MAX_SIZE)
	{
		//save new key in the key array
		key[size] = newKey;
		//save new value in the value array
		val[size] = newVal;
		//increase the size of the arrays
		size++;
		//heapifyUp at the instance
		heapifyUp(size - 1);
	}
}

template<typename T, typename U>
U Heap<T, U>::pop()
{
	//get first key
	T temp = key[0];
	//get first value
	U temp2 = val[0];
	//last key = first key
	key[0] = key[size - 1];
	//last value = first value
	val[0] = val[size - 1];
	//remove last key
	key[size - 1] = NULL;
	//remove last value
	val[size - 1] = NULL;
	//decrement size to change the size of the arrays
	size--;
	//heapify down at increment 0
	heapifyDown(0);
	//return value
	return temp2;				
}

template<typename T, typename U>
Heap<T, U>::Heap()
{
}

template<typename T, typename U>
Heap<T, U>::~Heap()
{

}

template<typename T, typename U>
void Heap<T, U>::heapifyUp(int i)
{
	//if item is on the top of the heap 
	if (i <= 0)
	{
		return;
	}
	//if not.. get parent
	int j = parent(i);
	//check if value of parent > child
	if (key[i] < key[j])
	{
		//if yes.. swap values/keys
		swap(i, j);
	}
	//heapify child
	heapifyUp(j);
}

template<typename T, typename U>
void Heap<T, U>::heapifyDown(int i)
{
	int j;
	//check if item has a child
	if (leftChild(i) > (size - 1))
	{
		return;
	}
	
	//check for right child 
	if (rightChild(i) > (size - 1))
	{
		
		j = leftChild(i);
	}
	//if item has both 
	else
	{
		//check values
		j = (key[leftChild(i)] < key[rightChild(i)]) ? (leftChild(i)) : (rightChild(i));
	}

	//if child < parent
	if (key[i] > key[j])
	{
		//swap
		swap(i, j);
	}
	heapifyDown(j);
}

template<typename T, typename U>
int Heap<T, U>::leftChild(int i)
{
	return (2 * i + 1);
}

template<typename T, typename U>
int Heap<T, U>::rightChild(int i)
{
	return (2 * i + 2);
}

template<typename T, typename U>
int Heap<T, U>::parent(int i)
{
	return ((i - 1) / 2);
}

template<typename T, typename U>
void Heap<T, U>::swap(int i, int j)
{
	//swap keys
	T temp = key[i];
	key[i] = key[j];
	key[j] = temp;

	//swap values
	U temp2 = val[i];
	val[i] = val[j];
	val[j] = temp2;
}
