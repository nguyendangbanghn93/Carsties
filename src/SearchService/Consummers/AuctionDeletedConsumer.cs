using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consummers
{
    public class AuctionDeletedConsumer : IConsumer<AuctionDeleted>
    {
        public async Task Consume(ConsumeContext<AuctionDeleted> context)
        {
            Console.WriteLine("--> Consuming AuctionDeleted: " + context.Message.Id);
            // Tại sao chỗ này nó tự hiểu DB ở đây là đối tượng DB trong project tôi đang sử dụng
            var result = await DB.DeleteAsync<Item>(context.Message.Id);

            if (!result.IsAcknowledged)
                throw new MessageException(typeof(AuctionDeleted), "Problem deleting auction");
        }
    }

}