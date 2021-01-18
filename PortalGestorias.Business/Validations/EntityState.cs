using System.Collections.Generic;

namespace PortalGestorias.Business.Validations
{
    public class EntityState
    {
        public EntityState()
        {
            Valid = true;
            ValidationErrors = new Dictionary<string, string>();
        }

        public bool Valid { get; set; }

        public IDictionary<string, string> ValidationErrors { get; set; }
    }

    public static class EntityStateExtensions
    {
        public static void AddRelatedEntityError(this EntityState state, string relatedEntity)
        {
            state.ValidationErrors.Add(
                string.Format(ErrorConstants.EntityRelatedNotActiveKey, relatedEntity),
                string.Format(ErrorConstants.EntityRelatedNotActive, relatedEntity));
        }
    }

    internal struct ErrorConstants
    {
        public const string EntityNotActiveKey = "EntityNotActive";

        public const string EntityNotActive = "La entidad no está activa y por lo tanto no puede ser modificada.";

        public const string EntityRelatedNotActiveKey = "EntityRelatedNotActive{0}";

        public const string EntityRelatedNotActive = "El {0} asociado seleccionado no está activo.";
    }
}