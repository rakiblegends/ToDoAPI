// API Base URL - Update this with your API URL
const API_BASE_URL = '/api/TodoTasks';

// State
let currentPage = 1;
const pageSize = 5;
let editingTaskId = null;
let tasks = [];

// DOM Elements
const taskForm = document.getElementById('taskForm');
const tasksList = document.getElementById('tasksList');
const noTasks = document.getElementById('noTasks');
const formTitle = document.getElementById('formTitle');
const cancelEdit = document.getElementById('cancelEdit');
const searchInput = document.getElementById('searchInput');
const searchButton = document.getElementById('searchButton');
const sortByPriority = document.getElementById('sortByPriority');
const sortByDueDate = document.getElementById('sortByDueDate');
const clearFilters = document.getElementById('clearFilters');
const pagination = document.getElementById('pagination');

// Event Listeners
document.addEventListener('DOMContentLoaded', () => {
    loadTasks();

    taskForm.addEventListener('submit', handleFormSubmit);
    cancelEdit.addEventListener('click', resetForm);
    searchButton.addEventListener('click', searchTasks);
    sortByPriority.addEventListener('click', () => loadTasksSorted('priority'));
    sortByDueDate.addEventListener('click', () => loadTasksSorted('duedate'));
    clearFilters.addEventListener('click', loadTasks);

    // Enter key in search input triggers search
    searchInput.addEventListener('keypress', (e) => {
        if (e.key === 'Enter') {
            searchTasks();
        }
    });
});

// Load Tasks
async function loadTasks() {
    try {
        showLoader();
        const response = await fetch(`${API_BASE_URL}/paged?pageNumber=${currentPage}&pageSize=${pageSize}`);
        if (!response.ok) throw new Error('Failed to fetch tasks');

        tasks = await response.json();
        renderTasks(tasks);
        updatePagination();
        hideLoader();
    } catch (error) {
        console.error('Error loading tasks:', error);
        showError('Failed to load tasks. Please try again later.');
        hideLoader();
    }
}

// Load Tasks with Sorting
async function loadTasksSorted(sortBy) {
    try {
        showLoader();
        const response = await fetch(`${API_BASE_URL}/${sortBy}`);
        if (!response.ok) throw new Error('Failed to fetch sorted tasks');

        tasks = await response.json();
        renderTasks(tasks);
        // No pagination for sorted results
        pagination.innerHTML = '';
        hideLoader();
    } catch (error) {
        console.error(`Error loading tasks sorted by ${sortBy}:`, error);
        showError('Failed to sort tasks. Please try again later.');
        hideLoader();
    }
}

// Search Tasks
async function searchTasks() {
    const searchTerm = searchInput.value.trim();
    if (!searchTerm) {
        loadTasks();
        return;
    }

    try {
        showLoader();
        const response = await fetch(`${API_BASE_URL}/search?title=${encodeURIComponent(searchTerm)}`);
        if (!response.ok) throw new Error('Failed to search tasks');

        tasks = await response.json();
        renderTasks(tasks);
        // No pagination for search results
        pagination.innerHTML = '';
        hideLoader();
    } catch (error) {
        console.error('Error searching tasks:', error);
        showError('Failed to search tasks. Please try again later.');
        hideLoader();
    }
}

// Render Tasks
function renderTasks(tasks) {
    tasksList.innerHTML = '';

    if (tasks.length === 0) {
        noTasks.classList.remove('d-none');
        return;
    }

    noTasks.classList.add('d-none');

    tasks.forEach(task => {
        const row = document.createElement('tr');
        row.onclick = () => editTask(task);

        const titleClass = task.isCompleted ? 'task-completed' : '';
        const dueDateFormatted = task.dueDate
            ? new Date(task.dueDate).toLocaleDateString('en-US', {
                year: 'numeric',
                month: 'short',
                day: 'numeric'
            })
            : '-';

        row.innerHTML = `
            <td class="${titleClass}">
                <strong>${escapeHtml(task.title)}</strong>
                ${task.description ? `<br><small class="text-muted">${truncateText(escapeHtml(task.description), 50)}</small>` : ''}
            </td>
            <td>${dueDateFormatted}</td>
            <td>
                <div class="priority-badge priority-${task.priority}">
                    ${task.priority}
                </div>
            </td>
            <td>
                <span class="badge ${task.isCompleted ? 'bg-success' : 'bg-warning text-dark'}">
                    ${task.isCompleted ? '<i class="fas fa-check-circle me-1"></i>Done' : '<i class="fas fa-clock me-1"></i>Pending'}
                </span>
            </td>
            <td>
                <button class="btn btn-sm btn-outline-primary me-1" onclick="editTask(${JSON.stringify(task).replace(/"/g, '&quot;')})">
                    <i class="fas fa-edit"></i>
                </button>
                <button class="btn btn-sm btn-outline-danger" onclick="event.stopPropagation(); deleteTask(${task.id})">
                    <i class="fas fa-trash"></i>
                </button>
            </td>
        `;

        tasksList.appendChild(row);
    });
}

// Handle Form Submit (Create or Update)
async function handleFormSubmit(e) {
    e.preventDefault();

    const taskData = {
        title: document.getElementById('title').value,
        description: document.getElementById('description').value,
        dueDate: document.getElementById('dueDate').value || null,
        priority: parseInt(document.getElementById('priority').value),
        isCompleted: document.getElementById('isCompleted').checked
    };

    try {
        showLoader();
        let response;

        if (editingTaskId) {
            // Update
            response = await fetch(`${API_BASE_URL}/${editingTaskId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(taskData)
            });
        } else {
            // Create
            response = await fetch(API_BASE_URL, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(taskData)
            });
        }

        if (!response.ok) throw new Error('Failed to save task');

        resetForm();
        loadTasks();
        showToast(editingTaskId ? 'Task updated successfully!' : 'Task created successfully!');

    } catch (error) {
        console.error('Error saving task:', error);
        showError('Failed to save task. Please try again.');
        hideLoader();
    }
}

// Edit Task
function editTask(task) {
    // Prevent click event issues
    if (typeof task === 'string') {
        task = JSON.parse(task);
    }

    editingTaskId = task.id;
    document.getElementById('taskId').value = task.id;
    document.getElementById('title').value = task.title;
    document.getElementById('description').value = task.description || '';
    document.getElementById('dueDate').value = task.dueDate ? new Date(task.dueDate).toISOString().split('T')[0] : '';
    document.getElementById('priority').value = task.priority;
    document.getElementById('isCompleted').checked = task.isCompleted;

    formTitle.innerHTML = '<i class="fas fa-edit me-2"></i>Edit Task';
    cancelEdit.classList.remove('d-none');

    // Scroll to form
    document.querySelector('.card').scrollIntoView({ behavior: 'smooth' });
}

// Delete Task
async function deleteTask(id) {
    if (!confirm('Are you sure you want to delete this task?')) return;

    try {
        showLoader();
        const response = await fetch(`${API_BASE_URL}/${id}`, {
            method: 'DELETE'
        });

        if (!response.ok) throw new Error('Failed to delete task');

        loadTasks();
        showToast('Task deleted successfully!');

    } catch (error) {
        console.error('Error deleting task:', error);
        showError('Failed to delete task. Please try again.');
        hideLoader();
    }
}

// Reset Form
function resetForm() {
    editingTaskId = null;
    taskForm.reset();
    formTitle.innerHTML = '<i class="fas fa-plus-circle me-2"></i>Add New Task';
    cancelEdit.classList.add('d-none');
}

// Update Pagination
function updatePagination() {
    // For simplicity, we're showing max 5 pages
    pagination.innerHTML = '';

    // Previous button
    const prevLi = document.createElement('li');
    prevLi.className = `page-item ${currentPage === 1 ? 'disabled' : ''}`;
    prevLi.innerHTML = `<a class="page-link" href="#" aria-label="Previous" onclick="changePage(${currentPage - 1})">
                            <span aria-hidden="true">&laquo;</span>
                         </a>`;
    pagination.appendChild(prevLi);

    // Page numbers
    for (let i = Math.max(1, currentPage - 2); i <= Math.min(currentPage + 2, 10); i++) {
        const li = document.createElement('li');
        li.className = `page-item ${i === currentPage ? 'active' : ''}`;
        li.innerHTML = `<a class="page-link" href="#" onclick="changePage(${i})">${i}</a>`;
        pagination.appendChild(li);
    }

    // Next button
    const nextLi = document.createElement('li');
    nextLi.className = 'page-item';
    nextLi.innerHTML = `<a class="page-link" href="#" aria-label="Next" onclick="changePage(${currentPage + 1})">
                            <span aria-hidden="true">&raquo;</span>
                         </a>`;
    pagination.appendChild(nextLi);
}

// Change Page
function changePage(page) {
    if (page < 1) return;
    currentPage = page;
    loadTasks();
}

// Helper function to escape HTML
function escapeHtml(unsafe) {
    return unsafe
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}

// Helper function to truncate text
function truncateText(text, maxLength) {
    return text.length > maxLength ? text.substring(0, maxLength) + '...' : text;
}

// Show error
function showError(message) {
    // Create toast container if it doesn't exist
    if (!document.getElementById('toastContainer')) {
        createToastContainer();
    }

    const toastContainer = document.getElementById('toastContainer');
    const toastId = 'error-' + Date.now();

    const toast = document.createElement('div');
    toast.className = 'toast';
    toast.setAttribute('role', 'alert');
    toast.setAttribute('aria-live', 'assertive');
    toast.setAttribute('aria-atomic', 'true');
    toast.setAttribute('id', toastId);
    toast.innerHTML = `
        <div class="toast-header bg-danger text-white">
            <strong class="me-auto"><i class="fas fa-exclamation-circle me-1"></i>Error</strong>
            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>
        <div class="toast-body">
            ${message}
        </div>
    `;

    toastContainer.appendChild(toast);

    const bsToast = new bootstrap.Toast(toast);
    bsToast.show();

    // Auto remove after shown
    toast.addEventListener('hidden.bs.toast', () => {
        toast.remove();
    });
}

// Show success toast
function showToast(message) {
    // Create toast container if it doesn't exist
    if (!document.getElementById('toastContainer')) {
        createToastContainer();
    }

    const toastContainer = document.getElementById('toastContainer');
    const toastId = 'toast-' + Date.now();

    const toast = document.createElement('div');
    toast.className = 'toast';
    toast.setAttribute('role', 'alert');
    toast.setAttribute('aria-live', 'assertive');
    toast.setAttribute('aria-atomic', 'true');
    toast.setAttribute('id', toastId);
    toast.innerHTML = `
        <div class="toast-header bg-success text-white">
            <strong class="me-auto"><i class="fas fa-check-circle me-1"></i>Success</strong>
            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>
        <div class="toast-body">
            ${message}
        </div>
    `;

    toastContainer.appendChild(toast);

    const bsToast = new bootstrap.Toast(toast);
    bsToast.show();

    // Auto remove after shown
    toast.addEventListener('hidden.bs.toast', () => {
        toast.remove();
    });
}

// Create toast container
function createToastContainer() {
    const toastContainer = document.createElement('div');
    toastContainer.id = 'toastContainer';
    toastContainer.className = 'toast-container position-fixed bottom-0 end-0 p-3';
    toastContainer.style.zIndex = '11';
    document.body.appendChild(toastContainer);
}

// Add loading indicator functions
function showLoader() {
    // Create loader if it doesn't exist
    if (!document.getElementById('globalLoader')) {
        const loader = document.createElement('div');
        loader.id = 'globalLoader';
        loader.className = 'd-none';
        loader.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(255, 255, 255, 0.7);
            display: flex;
            justify-content: center;
            align-items: center;
            z-index: 9999;
        `;

        loader.innerHTML = `
            <div class="spinner-border text-primary" role="status" style="width: 3rem; height: 3rem;">
                <span class="visually-hidden">Loading...</span>
            </div>
        `;

        document.body.appendChild(loader);
    }

    document.getElementById('globalLoader').classList.remove('d-none');
}

function hideLoader() {
    const loader = document.getElementById('globalLoader
