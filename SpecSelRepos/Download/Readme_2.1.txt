========================
Indicator Species Selection
========================

Version 2.0, 2018-03-21
Developed by Taoyang Wu <taoyang.wu@uea.ac.uk> for an objective, niche-based approach to indicator species selection proposed by Simon Butler <Simon.J.Butler@uea.ac.uk> etc (see Ref 1 below).
Further developed by Stephen Whiddett <s.whiddett@uea.ac.uk> to add investigative functionality and a GUI.

**********************************************************************************************

The input of the program is a text file containing the information for species and indicators. Columns 1 and 3 contain positive integer values representing species and resources respectively. Columns 2 and 4 contain positive integer or decimal values representing reliance and sensitivity respectively. In an option C file, column 5 contains positive integer or double values representing areas or precisions. All subsequent column values must be 1 or 0 indicating whether the species uses the resource (1 being positive).
The first row of the file must contain column headers of the format specifically defined below. Note that as of v2.0, naming species with strings is not allowed.

An example data input file for use with Options A or B might contain:
Species	Reliance Resources Sensitivity R1 R2 R3 R4 R5 R6
1	3	6	18	1	1	1	1	1	1
2   2	1	2	1	0	0	0	1	0
3	3	2	6	0	0	1	1	0	1
4	3	2	16	0	1	1	1	0	0
5	3	2	6	0	0	1	1	0	1
6	3	2	16	0	1	1	1	0	0
...

An example data input file for use with Option C might contain:
Species	Reliance Resources Sensitivity Area R1 R2 R3 R4 R5 R6
1	3	6	18	60000	1	1	1	1	1	1
2   2	1	2	5000	1	0	0	0	1	0
3	3	2	6	20000	0	0	1	1	0	1
4	3	2	16	8000	0	1	1	1	0	0
5	3	2	6	20000	0	0	1	1	0	1
6	3	2	16	8000	0	1	1	1	0	0
...

Data input files should only be used where the header line contains the following title formats:
Species	Reliance Resources Sensitivity R1 R2 R3 R4 R5 ... Rn			(Option A or B only)
Species	Reliance Resources Sensitivity Area R1 R2 R3 R4 R6 ... Rn		(Option C only)
Species	Reliance Resources Sensitivity Precision R1 R2 R3 R4 R6 ... Rn	(Option C only)

Each row of data should contain integers only and have the same number of integer values as there are header strings.
Species numbers should never repeat but need not be sequential.
All indicators of resource usage must be a 1 or 0.

The output is written to a text file named 'inputFileName_result.txt' where 'inputFileName' is the name of the file containing the processed data. For each size, that is, the number of species a species set may have, a collection of optimal (and suboptimal) species sets are given, in which the species sets are ordered by the maximal sensitivity followed by average sensitivity. For each species set, the maximal sensitivity and average sensitivity are also presented. For instance, an output 105:57.00:[2, 5] represents a species set consisting of two species, 2 and 5, with maximal sensitivity being 105 and average sensitivity being 57. 

To run the program, double click on the file "SpeciesSelection.jar". This will launch the GUI.

**********************************************************************************************

The 'Process' tab enables use of the features that were available in previous versions of the software. 
Process option A processes the full dataset by the standard method developed in Ref 1. The 'truncate results' option allows early termination of the results output at a point where the minimum mean sensitivity has increased in 3 consecutive result set sizes.

Option B enables processing using trait based exclusion criteria. Values for Species Threshold, 'm', and SD Threshold, 'x', may be set. Values of zero are ignored. 
Value set for 'm' ---> Uses trait based exclusion criteria, where traits used by less than m species are excluded. Here m is an integer
Value set for 'x' ---> Uses trait based exclusion criteria, where traits that are greater than x standard deviations from the average community sensitivity are excluded. Here x is a decimal number.
Value set for 'm' and 'x' ---> Uses trait based exclusion criteria, where traits that are used by less than m species AND are greater than x standard deviations from the average community sensitivity are excluded.

Option BF will use an imposed sensitivity. 
Option BN will use the normalised sensitivity.
Option BFN will use a normalised imposed sensitivity, while still applying values set for 'm' and/or 'x'.


Option C enables use of area OR precision based criteria, where species with area or precision less than Area/Prec Threshold, 'y', are excluded. Here 'y' is a decimal number. 
Note here that the input file needs one additional column to specify areas (or precision), see the file "testAreas.txt" included.
Using option C will first exclude the species with area less than 'y', and then use trait based exclusion criteria according to values set for 'm' and 'x'. 
Options 'CN', 'CF' or "CFN" can be used for imposed/normalised sensitivity as before.

**********************************************************************************************

The 'Analyse' tab is for use in identification of species in a dataset that cause greater than exponential growth in the processing time for a dataset, as each species is added to the dataset 1 by 1. The initial number of species to use, 'n', must be specified. 'n' should usually be set to a value where the initial subset is processed in no more than a few seconds. Allowable % Exp divergence must also be set. It is recommended that this value is set to a low value, such as 1, for an initial run, and is then increased for subsequent runs if too many marginally 'problem species' are identified.
After clicking 'Find Problem Species', a subset of the first 'n' species in the dataset will be processed and the MinSpecSetFamily (MSSF) size is recorded. The next species in the dataset is added to the subset and processed to again record MSSF size. An exponential curve is fitted to the data points at each step (after the first 3 subsets have been processed) which allows prediction of MSSF size when a subsequent species is added to the dataset. Should actual MSSF size be greater than (((100 + allowable % exp divergence)/100) * predicted MSSF size), the species is identified as a 'problem species' and left out of subsequent subsets. This expediates processing and results in a list of species that may make processing of the full dataset impractical.
Output may be produced for none, all or just the final subset to be run. Outputting results is processing intensive and so it is recommended to output results only where required by selection of the appropriate option.
All processing on this tab uses option A.

**********************************************************************************************

The 'Probability' tab allows the processing of a specified number of randomly generated subsets from an input dataset to be processed. All subsets must match the resource coverage (universe) of the full dataset, so specifying a subset size that is too low may mean that subsets cannot be found or take too long to find. If 'Max Time' is exceeded during subset generation, the program will abort and output the subsets found so far. The generated subsets may be run.
Truncation may be used, as in the Process tab, but the truncation threshold (number of min mean sensitivity increases) may be specified. A probability threshold may also be specified.
Running the subsets allows generation of output that identifies the probability that a species will appear in an optimal result set, of each set size, and therefore allows identification of species that never appear in an optimal solution. Exclusion of those improbable species may enable processing where it was previously impractical due to the length of processing time of the full dataset.
By default, the probabilities calculated are written out to 'probabilities.txt' but unchecked the overwrite option allows a filename to be specified. At the bottom of this output file will be a list of species with probability of inclusion in an optimal result set greater than the probability threshold specified.
A second output file is produced named 'inputFileName_ProbsSet.txt' where 'inputFileName' is the name of the file containing the processed data. This file contains the subset of species identified above ready for subsequent processing.
All processing on this tab uses option A.

**********************************************************************************************

Ref 1: Wade, A.S.I,  Barov, B.,  Burfield, I.J.,  Gregory, R.D.,  Norris, K., Wu, T., and Butler S.J. (2014) A niche-based framework to assess current monitoring of European forest birds and guide indicator species' selection.
PLoS One, 9(5): e97217 

Ref 2: "An objective, niche-based approach to indicator species selection" by Simon J. Butler etc. in "Methods in Ecology and Evolution" 2012. 
