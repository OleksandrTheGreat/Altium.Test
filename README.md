## Description
Implementation of a test task for Altium.
 
* Used IDE: Visual Studio 2017
* Language: C# for .Net
* Framework: .Net Core 2.0 

## Solution 

Solution consists on 5 modules:

0) Common utilities used accross solution
  * Altium.Test.Common

1) Client console application
  * Altium.Test.Console
  
2) Scenarios to execute independently for different UI implementations
  * Altium.Test.Scenarios
  * Altium.Test.Scenarios.Api 
  
2) Test file generator
  * Altium.Test.Generator
  * Altium.Test.Generator.api

3) Test file sorter
  * Altium.Test.Sorter
  * Altium.Test.Sorter.api

## Use
Build the solution and run /altium.test.file.console/bin/Debug/netcoreapp2.0/win10-x64/altium.test.file.console.exe 
Follow the instructions to perform Generate or Sort operations.

## Sort routine algorithm
0) Read target file lines to memory buffer
1) Group lines
2) Sort grouped lines
3) If memory buffer contains more lines then a block size, then drop the overhead to a buffer file
4) If reached end of target file then write lines from memory buffer to output file else goto [0]
5) If buffer file is not empty set target to buffer file and goto [0] else delete buffer file
7) Finish


## Some of test results

#1G

* Generate: 
- 00:00:01:762 (one repeating line)
- 00:00:01:304 (8% of repeating lines)
- 00:00:04:615 (all unique lines)
* Sort:
- 00:00:32:047 (one repeating line)
- 00:01:57:946 (8% of repeating lines)
- 00:01:38:398 (all unique lines)

#10G 
* Generate: 
- 00:00:57:338 (one repeating line)
- 00:00:50:356 (8% of repeating lines)
- (all unique lines)
* Sort:
- 00:03:14:760 (one repeating line)
- 00:03:14:703 (8% of repeating lines)
- (all unique lines)

#100G
* Generate:
* Sort: 
