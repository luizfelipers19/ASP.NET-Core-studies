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

        //retorna os detalhes de uma única reserva
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public IActionResult GetReservationDetail(int id)
        {
            //usando a sintaxe SQL para juntar as tabelas de reservas com a dos usuários, ligando a chave estrangeira à chave primária correspondente,
            // e depois juntando a tabela de filmes com a tabela de reservas.
            // após a junção das tabelas, retornar os atributos desejados com SELECT
            var reservationDetails = (from reservation in _dbContext.Reservations
                               join customer in _dbContext.Users on reservation.UserId equals customer.Id
                               join movie in _dbContext.Movies on reservation.MovieId equals movie.Id
                               where reservation.Id == id
                               select new
                               {
                                   Id = reservation.Id,
                                   ReservationTime = reservation.ReservationTime,
                                   CustomerName = customer.Name,
                                   MovieName = movie.Name,
                                   Email = customer.Email,
                                   Quantity = reservation.Quantity,
                                   Price = reservation.Price,
                                   Phone = reservation.Phone,
                                   PlayingDate = movie.PlayingDate,
                                   PlayingTime = movie.PlayingTime
                               }).FirstOrDefault();

            return Ok(reservationDetails);
        }


        // DELETE api/<MoviesController>/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {

            var reservation = _dbContext.Reservations.Find(id);

            // caso o objeto não exista/ não tenha sido encontrado pelo Id passado na requisição
            if (reservation == null)
            {
                return NotFound("Mano, tu tá tentando deletar um negócio de um registro que nem existe. Tenta ver certinho aí o número do Id da Reserva que vc tá passando, porque aqui no Servidor não tem nada com esse número");
            }

            else// caso o registro exista
            {
                _dbContext.Reservations.Remove(reservation);
                _dbContext.SaveChanges();

                return Ok("Registro Deletado mlk");


            }


        }
    }
}
