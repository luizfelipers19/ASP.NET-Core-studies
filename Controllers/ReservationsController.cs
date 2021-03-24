using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaAPI.Data;
using CinemaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CinemaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {

        private CinemaDbContext _dbContext;

        public ReservationsController(CinemaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize]
        [HttpPost]
        public IActionResult Post([FromBody]Reservation reservationObj)
        {

            reservationObj.ReservationTime = DateTime.Now;

            _dbContext.Reservations.Add(reservationObj);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);

        }


        [Authorize(Roles ="Admin")]
        [HttpGet]
        public IActionResult GetReservations()
        {
            //usando a sintaxe SQL para juntar as tabelas de reservas com a dos usuários, ligando a chave estrangeira à chave primária correspondente,
            // e depois juntando a tabela de filmes com a tabela de reservas.
            // após a junção das tabelas, retornar os atributos desejados com SELECT
             var reservations = from reservation in _dbContext.Reservations
            join customer in _dbContext.Users on reservation.UserId equals customer.Id
            join movie in _dbContext.Movies on reservation.MovieId equals movie.Id
            select new
            {
                Id = reservation.Id,
                ReservationTime = reservation.ReservationTime,
                CustomerName = customer.Name,
                MovieName = movie.Name
            };

            return Ok(reservations);
        }
    }
}
