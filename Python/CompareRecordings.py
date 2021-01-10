import csv

def writeArr(arr, name):
	try:
		file = open(name, "w")
		file.write(arr)

	finally:
		file.close()
		return

def compareWrite(arr1, arr2, name):
	arrOut = []
	z = 0

	if (arr1 == arr2):
		return true
	else:
		for (x in range(0, arr1.len)):
			for (y in range(0, arr2.len)):
				if (arr1[x] > arr2[y]):
					arrOut[z] = arr2[y]
					x -= 1
					z += 1
				elif (arr1[x] < arr2y]):
					arrOut[z] = arr1[x]
					y -= 1
					z += 1
		print "We are out of the loop, right? Printing..."
		writeArr(arrOut, name)
	return false

def main():
	try:
		ubounty = open("ubounty.csv", "rb")
		samsung = open("samsung.csv", "rb")
		tv = open("tv.csv", "rb")
		arrUbounty = csv.reader(ubounty)
		arrSamsung = csv.reader(samsung)
		arrTv = csv.reader(tv)

		compareWrite(ubounty, samsung, "uvs.txt")
		compareWrite(ubounty, tv, "uvt.txt")
		compareWrite(tv, samsung, "tvs.txt")		

	finally:
		ubounty.close()
		samsung.close()
		tv.close()
		return

main()