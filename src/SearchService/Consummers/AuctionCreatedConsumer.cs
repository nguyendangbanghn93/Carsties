using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consummers
{
    public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
    {
        private IMapper _mapper;

        public AuctionCreatedConsumer(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
            System.Console.WriteLine(
                "----------------->> Consuming auction created: " + context.Message.Id
            );

            var item = _mapper.Map<Item>(context.Message);

            if (item.Model == "Foo")
                throw new ArgumentException("Cannot sell cars with name of Foo");
            // khi tôi throw ntn thì việc retry có sảy ra hay không
            await item.SaveAsync();
        }
    }
}
