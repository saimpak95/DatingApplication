export interface Pagination {
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages: number;
}

export class Paginationresult<T> {
    result: T;
    pagination: Pagination;
}
