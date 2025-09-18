namespace CanvasTools.Connection;

/// <summary>
/// A connection to a Canvas installation.
/// </summary>
public interface IConnection
{
    /// <summary>
    /// Retrieves a list of entities fom the Canvas installation.
    /// </summary>
    /// <typeparam name="TItem">The type of item to retrieve.</typeparam>
    /// <param name="url">The URL to use.</param>
    /// <param name="parameters">The parameters to pass.</param>
    /// <param name="options">The options for retrieving the data.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to use.</param>
    /// <returns>An <see cref="IQueryable{TItem}"/> containing the entities from Canvas.</returns>
    IAsyncEnumerable<TItem> List<TItem>(string url, IParameters parameters, ListOptions? options = null, CancellationToken cancellationToken = default)
        where TItem : class;

    /// <summary>
    /// Retrieves an entity from the Canvas installation.
    /// </summary>
    /// <typeparam name="TItem">The type of item to retrieve.</typeparam>
    /// <param name="url">The URL to use.</param>
    /// <param name="parameters">The parameters to pass.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to use.</param>
    /// <returns>A <see cref="TItem"/> instance if found; <see langword="null" /> otherwise.</returns>
    Task<TItem?> Retrieve<TItem>(string url, IParameters parameters, CancellationToken cancellationToken = default)
        where TItem : class;
}