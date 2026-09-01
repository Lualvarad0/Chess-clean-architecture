namespace Ajedrez.Domain.SeedWork;

public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _eventosDominio = [];

    public IReadOnlyList<IDomainEvent> EventosDominio => _eventosDominio.AsReadOnly();

    protected void AgregarEvento(IDomainEvent evento) => _eventosDominio.Add(evento);

    public void LimpiarEventos() => _eventosDominio.Clear();
}
