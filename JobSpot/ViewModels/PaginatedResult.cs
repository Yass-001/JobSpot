namespace JobSpot.ViewModels
{
    /// <summary>
    /// Generic pagination result wrapper for any data type
    /// Encapsulates paginated data along with metadata for UI rendering
    /// </summary>
    /// <typeparam name="T">Type of items being paginated</typeparam>
    public class PaginatedResult<T>
    {
        /// <summary>
        /// The items for the current page
        /// </summary>
        public List<T> Items { get; set; } = new();

        /// <summary>
        /// Total count of all items (across all pages)
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Current page number (1-based)
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Calculated total pages
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        /// <summary>
        /// Whether there is a previous page available
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// Whether there is a next page available
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// Starting item number for this page (1-based)
        /// </summary>
        public int StartItemNumber => (PageNumber - 1) * PageSize + 1;

        /// <summary>
        /// Ending item number for this page
        /// </summary>
        public int EndItemNumber => Math.Min(PageNumber * PageSize, TotalCount);

        /// <summary>
        /// Get the previous page number, or null if on first page
        /// </summary>
        public int? PreviousPageNumber => HasPreviousPage ? PageNumber - 1 : null;

        /// <summary>
        /// Get the next page number, or null if on last page
        /// </summary>
        public int? NextPageNumber => HasNextPage ? PageNumber + 1 : null;
    }
}
