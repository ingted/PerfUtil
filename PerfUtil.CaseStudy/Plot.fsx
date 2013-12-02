﻿#r "bin/Release/FsPickler.dll"
#r "bin/Release/PerfUtil.dll"
#r "bin/Release/PerfUtil.CaseStudy.dll"

#load "../packages/FSharp.Charting.0.90.5/FSharp.Charting.fsx"


open PerfUtil
open PerfUtil.CaseStudy

open FSharp.Charting

// simple plot function
let plot yaxis (metric : PerfResult -> float) (results : PerfResult list) =
    let values = results |> List.choose (fun r -> if r.HasFailed then None else Some (r.SessionId, metric r))
    let name = results |> List.tryPick (fun r -> Some r.TestId)
    let ch = Chart.Bar(values, ?Name = name, ?Title = name, YTitle = yaxis)
    ch.ShowChart()


// read performance tests from 'Tests' module and run them
let results =
    PerfTest<ISerializer>.OfModuleMarker<Tests.Marker>()
    |> PerfTest.run SerializerComparer.Create

// plot everything
results
|> TestSession.groupByTest
|> Map.iter (fun _ r -> plot "milliseconds" (fun r -> r.Elapsed.TotalMilliseconds) r)