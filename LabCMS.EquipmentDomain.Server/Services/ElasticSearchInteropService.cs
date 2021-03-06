﻿using LabCMS.EquipmentDomain.Shared.Models;
using Microsoft.Extensions.Configuration;
using Nest;
using Elasticsearch.Net;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LabCMS.EquipmentDomain.Server.Services
{
    public class ElasticSearchInteropService
    {
        
        private readonly ElasticClient _elasticClient;
        public string IndexName { get; }
        public ElasticSearchInteropService(
            IConfiguration configuration)
        {
            _elasticClient = new(new Uri(configuration.GetConnectionString("ElasticSearchUrl")));

            string? usageRecordsIndex = configuration["UsageRecordsIndex"];
            if(usageRecordsIndex is null) { usageRecordsIndex = "usagerecords-actual"; }
            IndexName = usageRecordsIndex;
            if(!ValidateIndex())
            { throw new InvalidOperationException($"Index: {IndexName} was not created in ES!"); }
        }
        private bool ValidateIndex() => _elasticClient.Indices.Exists(IndexName).Exists;

        public async ValueTask IndexAsync(UsageRecord usageRecord)=>
            await _elasticClient.IndexAsync(usageRecord, item => item.Index(IndexName));
        
        public async ValueTask IndexManyAsync(IEnumerable<UsageRecord> usageRecords) =>
            await _elasticClient.IndexManyAsync(usageRecords, IndexName);

        public async ValueTask<IEnumerable<UsageRecord>> SearchAllAsync()
        {
            long size = (await _elasticClient.CountAsync<UsageRecord>(q=>q.Index(IndexName))).Count;
            return (await _elasticClient.SearchAsync<UsageRecord>(s => s.Index(IndexName).MatchAll().Size((int)size))).Documents;
        }

        public async ValueTask RemoveByIdAsync(Guid id) {
            var res =await _elasticClient.DeleteAsync<UsageRecord>(id,item=>item.Index(IndexName));
        }

        public async ValueTask RemoveManyAsync(IEnumerable<UsageRecord> usageRecords) =>
            await _elasticClient.DeleteManyAsync(usageRecords, IndexName);
        public async ValueTask RemoveAllAsync() =>
            await _elasticClient.Indices.DeleteAsync(new DeleteIndexRequest(IndexName));

    }
}
