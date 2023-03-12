using AngularEshop.Core.Services.Interfaces;
using AngularEshop.DataLayer.Entities.Site;
using AngularEshop.DataLayer.Ripository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularEshop.Core.Services.Implementations
{
    public class SliderService : ISliderService
    {
        #region constructor
        private IGenericRipository<Slider> sliderRipository;
        public SliderService(IGenericRipository<Slider> sliderRipository)
        {
            this.sliderRipository = sliderRipository;
        }

    
        #endregion
        #region Slider

        public async Task<List<Slider>> GetActiveSliders()
        {
            return await sliderRipository.GetEntitiesQuery().Where(s => !s.IsDelete).ToListAsync();
        }

        public async Task<List<Slider>> GetAllSliders()
        {
            return await sliderRipository.GetEntitiesQuery().ToListAsync();
        }
        public async Task AddSlider(Slider slider)
        {
            await sliderRipository.AddEntity(slider);
            await sliderRipository.SaveChanges();

        }
        public async Task<Slider> GetSliderById(long sliderId)
        {
            return await sliderRipository.GetEntityById(sliderId);
        }

        public async Task UpdateSlider(Slider slider)
        {
            sliderRipository.UpdateEntity(slider);
            await sliderRipository.SaveChanges();
        }
        #endregion
        #region Dispose
        public void Dispose()
        {
            sliderRipository?.Dispose();
        }

        #endregion

    }
}
