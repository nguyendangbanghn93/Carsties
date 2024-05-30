using ActionService.Entities;
using AuctionService.Data;
using AuctionService.DTOs;
using AutoMapper;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [Route("api/auctions")]
    public class AuctionsController : ControllerBase
    {
        public AuctionsController(
            AuctionDbContext context,
            IMapper mapper,
            IPublishEndpoint publishEndpoint
        )
        {
            _context = context;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        private readonly AuctionDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        [HttpGet]
        public async Task<List<AuctionDto>> GetAllAuctions()
        {
            var auctions = await _context
                .Auctions.Include(x => x.Item)
                .OrderBy(x => x.Item.Make)
                .ToListAsync();

            return _mapper.Map<List<Auction>, List<AuctionDto>>(auctions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
        {
            var auction = await _context
                .Auctions.Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null)
                return NotFound();

            return _mapper.Map<AuctionDto>(auction);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(
            [FromBody] CreateAuctionDto createAuctionDto
        )
        {
            var auction = _mapper.Map<Auction>(createAuctionDto);
            // TODO: add current user as seller
            auction.Seller = User.Identity.Name;

            _context.Auctions.Add(auction);

            var newAuction = _mapper.Map<AuctionDto>(auction);

            await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

            var result = await _context.SaveChangesAsync() > 0;

            if (!result)
                return BadRequest("Could not save changes to the DB");

            return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, newAuction);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuction(
            Guid id,
            [FromBody] UpdateAuctionDto updateAuctionDto
        )
        {
            var auction = await _context
                .Auctions.Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null)
                return NotFound();

            if (auction.Seller != User.Identity.Name)
                return Forbid();
            // Logging dữ liệu nhận được

            auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
            auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
            auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

            await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));

            var result = await _context.SaveChangesAsync() > 0;

            if (!result)
                return BadRequest("Problem saving changes");

            return Ok();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction = await _context.Auctions.FindAsync(id);

            if (auction == null)
                return NotFound();

            if (auction.Seller != User.Identity.Name)
                return Forbid();

            _context.Auctions.Remove(auction);

            await _publishEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString() });

            var result = await _context.SaveChangesAsync() > 0;

            if (!result)
                return BadRequest("Could not update DB");

            return Ok();
        }
    }
}
