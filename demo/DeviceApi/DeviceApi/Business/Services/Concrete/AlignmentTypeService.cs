using AutoMapper;
using Data.Interfaces;
using DeviceApi.Business.Services.Interfaces;
using Entities.Concrete;
using Entities.Dtos;
using LogLibrary.Core.Interfaces;

namespace DeviceApi.Business.Services.Concrete
{
    /// <summary>
    /// Hizalama türleri servisi implementasyonu
    /// </summary>
    public class AlignmentTypeService : IAlignmentTypeService
    {
        private readonly IAlignmentTypeRepository _alignmentTypeRepository;
        private readonly ILogService _logService;
        private readonly IMapper _mapper;

        public AlignmentTypeService(
            IAlignmentTypeRepository alignmentTypeRepository,
            ILogService logService,
            IMapper mapper)
        {
            _alignmentTypeRepository = alignmentTypeRepository;
            _logService = logService;
            _mapper = mapper;
        }

        /// <summary>
        /// Tüm hizalama türlerini getirir
        /// </summary>
        public async Task<List<AlignmentTypeDto>> GetAllAlignmentTypesAsync()
        {
            var alignmentTypes = await _alignmentTypeRepository.GetAllAlignmentTypesAsync();
            return _mapper.Map<List<AlignmentTypeDto>>(alignmentTypes);
        }

        /// <summary>
        /// ID'ye göre hizalama türü getirir
        /// </summary>
        public async Task<AlignmentTypeDto> GetAlignmentTypeByIdAsync(int id)
        {
            var alignmentType = await _alignmentTypeRepository.GetAlignmentTypeByIdAsync(id);
            if (alignmentType == null)
            {
                throw new Exception("Hizalama türü bulunamadı");
            }
            
            return _mapper.Map<AlignmentTypeDto>(alignmentType);
        }

        /// <summary>
        /// Key değerine göre hizalama türü getirir
        /// </summary>
        public async Task<AlignmentTypeDto> GetAlignmentTypeByKeyAsync(int key)
        {
            var alignmentType = await _alignmentTypeRepository.GetAlignmentTypeByKeyAsync(key);
            if (alignmentType == null)
            {
                throw new Exception("Belirtilen key değerine sahip hizalama türü bulunamadı");
            }
            
            return _mapper.Map<AlignmentTypeDto>(alignmentType);
        }

        /// <summary>
        /// Hizalama türü oluşturur
        /// </summary>
        public async Task<AlignmentTypeDto> CreateAlignmentTypeAsync(CreateAlignmentTypeRequest request)
        {
            // Aynı key değerine sahip başka bir türün olup olmadığını kontrol et
            var existingType = await _alignmentTypeRepository.GetAlignmentTypeByKeyAsync(request.Key);
            if (existingType != null)
            {
                throw new Exception($"Bu key değeri ({request.Key}) zaten kullanılıyor");
            }
            
            var alignmentType = _mapper.Map<AlignmentType>(request);
            
            await _alignmentTypeRepository.AddAlignmentTypeAsync(alignmentType);
            
            await _logService.LogInfoAsync(
                "Hizalama türü oluşturuldu", 
                "AlignmentTypeService.CreateAlignmentType", 
                new { AlignmentTypeId = alignmentType.Id, Key = alignmentType.Key });
            
            return _mapper.Map<AlignmentTypeDto>(alignmentType);
        }

        /// <summary>
        /// Hizalama türü günceller
        /// </summary>
        public async Task<AlignmentTypeDto> UpdateAlignmentTypeAsync(int id, UpdateAlignmentTypeRequest request)
        {
            var existingType = await _alignmentTypeRepository.GetAlignmentTypeByIdAsync(id);
            if (existingType == null)
            {
                throw new Exception("Güncellenecek hizalama türü bulunamadı");
            }
            
            // Key değeri değiştiriliyorsa çakışma olup olmadığını kontrol et
            if (existingType.Key != request.Key)
            {
                var keyExists = await _alignmentTypeRepository.GetAlignmentTypeByKeyAsync(request.Key);
                if (keyExists != null && keyExists.Id != id)
                {
                    throw new Exception($"Bu key değeri ({request.Key}) zaten başka bir tür tarafından kullanılıyor");
                }
            }
            
            _mapper.Map(request, existingType);
            
            await _alignmentTypeRepository.UpdateAlignmentTypeAsync(existingType);
            
            await _logService.LogInfoAsync(
                "Hizalama türü güncellendi", 
                "AlignmentTypeService.UpdateAlignmentType", 
                new { AlignmentTypeId = id });
            
            return _mapper.Map<AlignmentTypeDto>(existingType);
        }

        /// <summary>
        /// Hizalama türü siler
        /// </summary>
        public async Task DeleteAlignmentTypeAsync(int id)
        {
            var existingType = await _alignmentTypeRepository.GetAlignmentTypeByIdAsync(id);
            if (existingType == null)
            {
                throw new Exception("Silinecek hizalama türü bulunamadı");
            }
            
            // İlişkili FullScreenMessage var mı kontrol et
            // TODO: FullScreenMessage repository'sinden kontrol eklenebilir
            
            await _alignmentTypeRepository.DeleteAlignmentTypeAsync(id);
            
            await _logService.LogInfoAsync(
                "Hizalama türü silindi", 
                "AlignmentTypeService.DeleteAlignmentType", 
                new { AlignmentTypeId = id });
        }
    }
} 