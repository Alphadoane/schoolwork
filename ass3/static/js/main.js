/**
 * main.js – Shared utilities for the ATM Management System.
 * Provides: API helpers, toast notifications, modal controls, and formatters.
 */

// ── Number Formatter ──────────────────────────────────────────────────────────
function fmt(num) {
    return Number(num).toLocaleString('en-KE', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
}

// ── Toast Notifications ───────────────────────────────────────────────────────
function showToast(message, type = 'info', duration = 4000) {
    const container = document.getElementById('toast-container');
    if (!container) return;

    const icons = { success: '✅', error: '❌', info: 'ℹ️' };
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    toast.innerHTML = `<span>${icons[type] || 'ℹ️'}</span><span>${message}</span>`;
    container.appendChild(toast);

    setTimeout(() => {
        toast.style.animation = 'slideOut 0.35s cubic-bezier(0.4,0,0.2,1) forwards';
        setTimeout(() => toast.remove(), 350);
    }, duration);
}

// ── API Helpers ───────────────────────────────────────────────────────────────
async function _fetch(url, options = {}) {
    try {
        const res = await fetch(url, {
            headers: { 'Content-Type': 'application/json', ...options.headers },
            credentials: 'same-origin',
            ...options,
        });
        const data = await res.json();
        return data;
    } catch (err) {
        console.error('API error:', err);
        return { error: 'Network error. Please try again.' };
    }
}

function apiGet(url) {
    return _fetch(url, { method: 'GET' });
}

function apiPost(url, body) {
    return _fetch(url, { method: 'POST', body: JSON.stringify(body) });
}

function apiPut(url, body) {
    return _fetch(url, { method: 'PUT', body: JSON.stringify(body) });
}

function apiDelete(url) {
    return _fetch(url, { method: 'DELETE' });
}

// ── Modal Controls ────────────────────────────────────────────────────────────
function openModal(id) {
    const overlay = document.getElementById(id);
    if (overlay) overlay.classList.add('active');
}

function closeModal(id) {
    const overlay = document.getElementById(id);
    if (overlay) overlay.classList.remove('active');
}

// Close modal on overlay click
document.addEventListener('click', (e) => {
    if (e.target.classList.contains('modal-overlay')) {
        e.target.classList.remove('active');
    }
});

// Close modal on Escape key
document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape') {
        document.querySelectorAll('.modal-overlay.active').forEach(m => m.classList.remove('active'));
    }
});
