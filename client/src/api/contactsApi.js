const BASE_PATH = '/api/contacts'

export class ApiError extends Error {
  constructor(message, status, fieldErrors = {}) {
    super(message)
    this.name = 'ApiError'
    this.status = status
    this.fieldErrors = fieldErrors
  }
}

function toCamelCase(fieldName) {
  return fieldName.charAt(0).toLowerCase() + fieldName.slice(1)
}

async function handleResponse(response) {
  if (response.ok) {
    if (response.status === 204) {
      return undefined
    }
    return response.json()
  }

  let body = null
  try {
    body = await response.json()
  } catch {
    body = null
  }

  if (response.status === 400 && body?.errors) {
    const fieldErrors = {}
    for (const [field, messages] of Object.entries(body.errors)) {
      fieldErrors[toCamelCase(field)] = Array.isArray(messages) ? messages[0] : messages
    }
    throw new ApiError(body.title ?? 'Validation failed.', 400, fieldErrors)
  }

  if (response.status === 404) {
    throw new ApiError('Contact not found.', 404)
  }

  if (response.status === 409) {
    const message = body?.message ?? 'Phone number already exists.'
    throw new ApiError(message, 409, { phoneNumber: message })
  }

  throw new ApiError(body?.title ?? response.statusText ?? 'Request failed.', response.status)
}

function buildQuery(params) {
  const query = new URLSearchParams()

  for (const [key, value] of Object.entries(params)) {
    if (value !== null && value !== undefined && value !== '') {
      query.set(key, value)
    }
  }

  const queryString = query.toString()
  return queryString ? `?${queryString}` : ''
}

export function getPaged({ page = 1, pageSize = 10, search = '' } = {}) {
  const query = buildQuery({ page, pageSize, search })
  return fetch(`${BASE_PATH}${query}`).then(handleResponse)
}

export function getById(id) {
  return fetch(`${BASE_PATH}/${id}`).then(handleResponse)
}

export function create(contact) {
  return fetch(BASE_PATH, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(contact),
  }).then(handleResponse)
}

export function update(id, contact) {
  return fetch(`${BASE_PATH}/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(contact),
  }).then(handleResponse)
}

export function remove(id) {
  return fetch(`${BASE_PATH}/${id}`, {
    method: 'DELETE',
  }).then(handleResponse)
}
