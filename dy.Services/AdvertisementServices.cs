using AutoMapper;
using dy.Common.Helper;
using dy.IRepository;
using dy.IServices;
using dy.Model.Test;
using dy.Repository;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace dy.Services
{
    public class AdvertisementServices : BaseServices<Advertisement>, IAdvertisementServices
    {
        private readonly IAdvertisementRepository _advertisementDal;
        private readonly IMapper iMapper;

        public AdvertisementServices(IBaseRepository<Advertisement> baseRepository, IAdvertisementRepository advertisemenRepository, IMapper IMapper) : base(baseRepository)
        {
            _advertisementDal = advertisemenRepository;
            iMapper = IMapper;
        }

        public async Task<bool> PostAdvertisementAsync(AdvertisementInput input)
        {
            Advertisement model = iMapper.Map<Advertisement>(input);
            model.Id = IdHelper.CreateGuid();
            model.ImgUrl = "www.baidu.com";
            return await _advertisementDal.PostAdvertisementAsync(model);
        }

    }
}
