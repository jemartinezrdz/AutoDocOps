namespace AutoDocOps.Domain.Entities;

/// <summary>
/// Entidad base que proporciona propiedades comunes para todas las entidades del dominio
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Identificador único de la entidad
    /// </summary>
    public Guid Id { get; protected set; } = Guid.NewGuid();

    /// <summary>
    /// Fecha y hora de creación de la entidad
    /// </summary>
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha y hora de la última actualización de la entidad
    /// </summary>
    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;

    /// <summary>
    /// Identificador del usuario que creó la entidad
    /// </summary>
    public Guid CreatedBy { get; protected set; }

    /// <summary>
    /// Identificador del usuario que actualizó la entidad por última vez
    /// </summary>
    public Guid UpdatedBy { get; protected set; }

    /// <summary>
    /// Indica si la entidad está activa o ha sido eliminada lógicamente
    /// </summary>
    public bool IsActive { get; protected set; } = true;

    /// <summary>
    /// Actualiza la fecha de modificación y el usuario que realizó la modificación
    /// </summary>
    /// <param name="updatedBy">Identificador del usuario que realiza la actualización</param>
    public void UpdateTimestamp(Guid updatedBy)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Realiza una eliminación lógica de la entidad
    /// </summary>
    /// <param name="deletedBy">Identificador del usuario que realiza la eliminación</param>
    public void SoftDelete(Guid deletedBy)
    {
        IsActive = false;
        UpdateTimestamp(deletedBy);
    }

    /// <summary>
    /// Restaura una entidad eliminada lógicamente
    /// </summary>
    /// <param name="restoredBy">Identificador del usuario que realiza la restauración</param>
    public void Restore(Guid restoredBy)
    {
        IsActive = true;
        UpdateTimestamp(restoredBy);
    }
}

