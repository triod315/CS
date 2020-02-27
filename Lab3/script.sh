#!/bin/bash

uind=`date | awk '{print $4}' | awk -F: '{print $2$3}'`
echo $uind
compiler_flags=(O0 O1 O2 O3 Os Ofast)

flagsout="{"
time_list="{"

for flag in ${compiler_flags[@]}
do
    filename=gpp$1$flag
    echo -$flag $1 -o $filename
    g++ -$flag $1 -o $filename -std=c++11
    T=`(time -p ./$filename $2 $3) 2>&1 | grep real | awk '{print ($2)'} | sed 's/,/./'`
    flagsout=$flagsout", "\"$flag\"
    time_list=$time_list", "$T
done


echo $flagsout}>"gnuresult$uind"
echo $time_list}>>"gnuresult$uind"
flagsout="{"
time_list="{"

icc_cpu_flags=(sse2 ssse3 sse4.1 sse4.2)
ml icc
for iccflag in ${icc_cpu_flags[@]}
do
    filename=icc$1$iccflag
    echo -x$iccflag $1 -o $filename
    icc -x$iccflag -Ofast $1 -o $filename -std=c++11
    T=`(time -p ./$filename $2 $3) 2>&1 | grep real | awk '{print ($2)'} | sed 's/,/./'`
    flagsout=$flagsout", "\"$iccflag\"
    time_list=$time_list", "$T
done

echo $flagsout}>intelresult$uind
echo $time_list}>>intelresult$uind

rm gp*
rm ic*
