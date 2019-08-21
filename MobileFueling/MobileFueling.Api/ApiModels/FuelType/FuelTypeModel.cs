using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MobileFueling.Api.Common.Localization;
using MobileFueling.DB;
using MobileFueling.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileFueling.Api.ApiModels.FuelType
{
    public class FuelTypeModel
    {
        private readonly FuelDbContext _fuelContext;
        private readonly IStringLocalizer _stringLocalizer;

        public FuelTypeModel(FuelDbContext fuelContext, IStringLocalizer stringLocalizer)
        {
            _fuelContext = fuelContext;
            _stringLocalizer = stringLocalizer;
        }

        public async Task<IEnumerable<FuelTypeVM>> GetAll()
        {
            var items = await _fuelContext.FuelTypes.ToListAsync();
            return items.Select(Convert);
        }

        public async Task<FuelTypeVM> GetOne(long id)
        {
            var item = await _fuelContext.FuelTypes.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
                throw new ArgumentException(_stringLocalizer[CustomStringLocalizer.FUEL_TYPE_CAN_NOT_FIND]);

            return Convert(item);
        }

        public async Task<long> PostOne(FuelTypeVM fuelTypeVM)
        {
            Model.FuelType fuelType = null;
            if (fuelTypeVM.Id.HasValue)
                fuelType = await _fuelContext.FuelTypes.FirstOrDefaultAsync(x => x.Id == fuelTypeVM.Id.Value);
            else
                fuelType = new Model.FuelType { };

            fuelType.Name = fuelTypeVM.Name;

            if (fuelTypeVM.Id.HasValue)
                _fuelContext.FuelTypes.Update(fuelType);
            else
                await _fuelContext.FuelTypes.AddAsync(fuelType);

            _fuelContext.SaveChanges();
            return fuelType.Id;
        }

        public async Task DeleteOne(long id)
        {
            var item = await _fuelContext.FuelTypes.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
                throw new ArgumentException(_stringLocalizer[CustomStringLocalizer.FUEL_TYPE_CAN_NOT_FIND]);

            _fuelContext.FuelTypes.Remove(item);
            await _fuelContext.SaveChangesAsync();
        }

        private FuelTypeVM Convert(Model.FuelType fuelType)
        {
            if (fuelType == null)
                return null;

            return new FuelTypeVM
            {
                Id = fuelType.Id,
                Name = fuelType.Name
            };
        }
    }
}