document.addEventListener('DOMContentLoaded', () => {
    
    const fadeInElements = document.querySelectorAll('.fade-in');

    fadeInElements.forEach(el => {
        el.style.animationDelay = '0.2s';
    });
});