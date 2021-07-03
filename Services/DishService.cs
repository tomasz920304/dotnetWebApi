using AutoMapper;
using dotnetWebApi.Entities;
using dotnetWebApi.Exceptions;
using dotnetWebApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetWebApi.Services
{
    public interface IDishService
    {
        int Create(int restaurantId, CreateDishDto dto);
        DishDto GetById(int restaurantId, int dishId);
        List<DishDto> GetAll(int restaurantId);
        void RemoveAll(int restaurantId);
    }

    public class DishService : IDishService
    {
        private readonly RestaurantDbContext _context;
        private readonly IMapper _mapper;

        public DishService(RestaurantDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<DishDto> GetAll(int restaurantId)
        {

            var restaurant = GetRestaurantById(restaurantId);
            var dishDtos = _mapper.Map<List<DishDto>>(restaurant.Dishes);

            return dishDtos;
        }

        public DishDto GetById(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            var dish = _context.Dishes.FirstOrDefault(d => d.Id == dishId);
            if (dish is null || dish.RestaurantId != restaurantId)
            {
                throw new NotFoundException("Dish not found");
            }

            var dishDto = _mapper.Map<DishDto>(dish);
            return dishDto;
        }

        public int Create(int restaurantId, CreateDishDto dto)
        {
            var restaurant = GetRestaurantById(restaurantId);
            var dishEntity = _mapper.Map<Dish>(dto);

            dishEntity.RestaurantId = restaurantId;

            _context.Dishes.Add(dishEntity);
            _context.SaveChanges();

            return dishEntity.Id;
        }

        public void RemoveAll(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            _context.RemoveRange(restaurant.Dishes);
            _context.SaveChanges();

        }

        private Restaurant GetRestaurantById(int restaurantId)
        {
            var restaurant = _context
                .Restaurants
                .Include(r => r.Dishes)
                .FirstOrDefault(r => r.Id == restaurantId);

            if (restaurant is null)
                throw new NotFoundException("Restaurant not found");

            return restaurant;
        }
    }
}
