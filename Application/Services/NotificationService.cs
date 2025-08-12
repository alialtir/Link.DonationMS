using Application.Services.Abstractions;
using AutoMapper;
using Domain.Contracts;
using Domain.Models;
using DTOs.NotificationDTOs;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NotificationService(
            IUnitOfWork unitOfWork, 
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CreateNotificationAsync(NotificationTypeId typeId, string to, object parameters, NotificationLanguage language = NotificationLanguage.Arabic)
        {
            // 1. Get notification type by TypeId and Language
            var notificationTypes = await _unitOfWork.NotificationTypes.GetAllAsync();
            var notificationType = notificationTypes.FirstOrDefault(nt => nt.TypeId == typeId && nt.LanguageId == language);

            if (notificationType == null)
                throw new ArgumentException($"Notification type with ID '{typeId}' not found.");

            // 2. Replace placeholders in Subject and Body
            var subject = ReplacePlaceholders(notificationType.Subject, parameters);
            var body = ReplacePlaceholders(notificationType.Body, parameters);

            // 3. Create Notification with correct data from template
            var notification = new Notification
            {
                To = to,
                Subject = subject, // Subject from template with replaced variables
                Body = body, // Body from template with replaced variables
                IsSent = false, // Not sent yet
                LanguageId = notificationType.LanguageId, // Language from template
                NotificationTypeId = (int)typeId, // Store enum value (1:Receipt,2:Goal,3:Register)
                UserId = ExtractUserId(parameters) // Extract UserId if exists
            };

            // 4. Add to database (without saving - let caller handle transaction)
            await _unitOfWork.Notifications.AddAsync(notification);
            // Note: CompleteAsync() should be called by the caller to control transaction
        }

        /// <summary>
        /// Replace placeholders in text templates
        /// </summary>
        private string ReplacePlaceholders(string template, object parameters)
        {
            if (parameters == null || string.IsNullOrEmpty(template))
                return template;

            foreach (var prop in parameters.GetType().GetProperties())
            {
                var placeholder = $"{{{{{prop.Name}}}}}";
                var value = prop.GetValue(parameters)?.ToString() ?? "";
                template = template.Replace(placeholder, value);
            }
            return template;
        }

        /// <summary>
        /// Extract UserId from parameters if exists
        /// </summary>
        private Guid? ExtractUserId(object parameters)
        {
            try
            {
                var userIdProp = parameters.GetType().GetProperty("UserId");
                if (userIdProp?.GetValue(parameters) is Guid userId)
                    return userId;
            }
            catch { }
            
            return null; // No UserId found
        }
    }
}
