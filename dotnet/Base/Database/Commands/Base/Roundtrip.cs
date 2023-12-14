﻿// <copyright file="Restore.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Commands
{
    using System.IO;
    using Allors.Database.Population;
    using McMaster.Extensions.CommandLineUtils;
    using NLog;

    [Command(Description = "Roundtrip records to file")]
    public class Roundtrip
    {
        public Program Parent { get; set; }

        public Logger Logger => LogManager.GetCurrentClassLogger();

        [Option("-f", Description = "records file")]
        public string FileName { get; set; }

        public int OnExecute(CommandLineApplication app)
        {
            this.Logger.Info("Begin");

            var fileName = this.FileName ?? this.Parent.Configuration["recordsFile"] ?? "../../../../Population/Records.xml";
            var fileInfo = new FileInfo(fileName);

            this.Logger.Info("Saving {file}", fileInfo.FullName);

            var database = this.Parent.Database;

            var recordsFromFile = new RecordsFromFile(fileInfo, database.MetaPopulation);
            var roundtrip = new RecordRoundtripStrategy(database, recordsFromFile.RecordsByClass);
            var recordsToFile = new RecordsToFile(fileInfo, database.MetaPopulation, roundtrip);
            recordsToFile.Roundtrip();

            this.Logger.Info("End");
            return ExitCode.Success;
        }
    }
}