export interface ValidationProblem {
    type?: string
    title?: string
    status?: number
    errors: Record<string, string[]>
}

export class ApiError extends Error {
    status: number

    constructor(status: number, message: string) {
        super(message)
        this.name = 'ApiError'
        this.status = status
    }
}

export class ValidationError extends ApiError {
    problem: ValidationProblem

    constructor(problem: ValidationProblem) {
        super(400, problem.title ?? 'Validation failed')
        this.name = 'ValidationError'
        this.problem = problem
    }
}

export class ConflictError extends ApiError {
    detail: string

    constructor(detail: string) {
        super(409, detail)
        this.name = 'ConflictError'
        this.detail = detail
    }
}

export class NotFoundError extends ApiError {
    constructor() {
        super(404, 'Not found')
        this.name = 'NotFoundError'
    }
}
