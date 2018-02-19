## Description
Implementation of a test task for Altium.
 
* Used IDE: Visual Studio 2017
* Language: C# for .Net
* Framework: .Net Core 2.0 

## Solution 

Solution consists on 4 main modules:

1) Client console application
  * altium.test.file.console
  
2) Scenarios to execute independently for different UI implementations
  * altium.test.file.scenarios
  * altium.test.file.scenarios.api 
  
2) Test file generator
  * altium.test.file.generator
  * altium.test.file.generator.api

3) Test file sorter
  * altium.test.file.sorter
  * altium.test.file.sorter.api

## Use
Build the solution and run /altium.test.file.console/bin/Debug/netcoreapp2.0/win10-x64/altium.test.file.console.exe 
Follow the instructions to perform Generate or Sort operations.

## Sort routine description
0) Read target file rows in parallel and fill dictionary where key is row and value is row hit counter
1) Parse results as set of objects {Number, String, Count, SourceLine}
2) Sort parsed set first by String then by Number using LINQ query
3) Write sorted set to file using object's SourceLine property object's Count number times to output file

This does not seem to be the best solution. However, this solution turned out to be faster and less expensive than using Merge-Sort algorithm or Database BULK insert and sort approaches.


## Some of test results

#1G
* Generated: 00:00:11:211
* Sorted:    00:01:27:395

#10G 
* Generated: 00:01:52:104
* Sorted:    00:03:43:491

#100G
* Generated: 00:18:19:330
* Sorted:    00:28:21:732
