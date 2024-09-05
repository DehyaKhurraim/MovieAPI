using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MovieAPI.Data;
using MovieAPI.Interface;
using MovieAPI.Models;
using Newtonsoft.Json;

namespace MovieAPI.Repository
{
    public class MovieRepository : IMovieRepository
    {
        private readonly ApplicationDbContext _context;
       
        public MovieRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Movie> AddMovieAsync(Movie movie)
        {
            await _context.Movies.AddAsync(movie);
            await _context.SaveChangesAsync();

            await UpdateCache();

            return movie;
        }

        public async Task<Movie?> DeleteMovieAsync(int id)
        {
            var Movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);

            if(Movie == null)
                return null;
            
            _context.Movies.Remove(Movie);
            await _context.SaveChangesAsync();

            await UpdateCache();

            return Movie;
        }

        public async Task<IEnumerable<Movie?>> GetAllMoviesAsync()
        {

          
            
            var movieList = await _context.Movies.ToListAsync();
            var serializedMovieList = JsonConvert.SerializeObject(movieList);

            

            return movieList;
        }

        public async Task<Movie?> GetMovieByIdAsync(int id)
        {
            var moviesList = await GetAllMoviesAsync();
            var movie = moviesList?.FirstOrDefault(m => m.Id == id);

            if(movie == null)
                return null;
            
            return movie;
        }

        public async Task<Movie?> UpdateMovieAsync(Movie movie, int id)
        {
            var oldMovie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);

            if(oldMovie == null)
                return null;      
                           
            oldMovie.Title = movie.Title;
            oldMovie.Description = movie.Description;
            oldMovie.Rating = movie.Rating;

            await _context.SaveChangesAsync();

            await UpdateCache();
            
            return oldMovie;
        }

        private async Task UpdateCache()
        {
            var moviesList = await _context.Movies.ToListAsync();
            var serializedMoviesList = JsonConvert.SerializeObject(moviesList);

        }
    }
}