document.addEventListener('DOMContentLoaded', function () {
    const slides = document.querySelectorAll('.hero-slide');
    if (slides.length === 0) return;

    let currentSlide = 0;

    setInterval(function () {
        slides[currentSlide].classList.remove('active');
        currentSlide = (currentSlide + 1) % slides.length;
        slides[currentSlide].classList.add('active');
    }, 4000);
});
//header.js
document.addEventListener('DOMContentLoaded', function () {
    const hamburgerBtn = document.getElementById('hamburgerBtn');
    const megaMenu = document.getElementById('megaMenu');
    const megaMenuClose = document.getElementById('megaMenuClose');
    const megaMenuOverlay = document.getElementById('megaMenuOverlay');

    function openMenu() {
        megaMenu.classList.add('open');
        document.body.style.overflow = 'hidden';
    }

    function closeMenu() {
        megaMenu.classList.remove('open');
        document.body.style.overflow = '';
    }

    hamburgerBtn.addEventListener('click', openMenu);
    megaMenuClose.addEventListener('click', closeMenu);
    megaMenuOverlay.addEventListener('click', closeMenu);
});

//rooms-filters.js
document.addEventListener('DOMContentLoaded', function () {
    const filterButtons = document.querySelectorAll('.filter-btn');
    const roomCards = document.querySelectorAll('#roomsGrid .room-card');

    if (filterButtons.length === 0) return;

    let activeFilters = new Set(['all']);

    filterButtons.forEach(function (btn) {
        btn.addEventListener('click', function () {
            const filter = btn.dataset.filter;

            if (filter === 'all') {
                activeFilters = new Set(['all']);
                filterButtons.forEach(b => b.classList.remove('active'));
                btn.classList.add('active');
            } else {
                activeFilters.delete('all');
                document.querySelector('.filter-btn[data-filter="all"]').classList.remove('active');

                if (activeFilters.has(filter)) {
                    activeFilters.delete(filter);
                    btn.classList.remove('active');
                } else {
                    activeFilters.add(filter);
                    btn.classList.add('active');
                }

                if (activeFilters.size === 0) {
                    activeFilters.add('all');
                    document.querySelector('.filter-btn[data-filter="all"]').classList.add('active');
                }
            }

            applyFilters();
        });
    });

    function applyFilters() {
        roomCards.forEach(function (card) {
            if (activeFilters.has('all')) {
                card.classList.remove('hidden');
                return;
            }

            const type = card.dataset.type;
            const capacity = parseInt(card.dataset.capacity);
            const price = parseFloat(card.dataset.price);

            let matches = false;

            if (activeFilters.has('vip') && type === 'vip') matches = true;
            if (activeFilters.has('large') && capacity >= 10) matches = true;
            if (activeFilters.has('cheap') && price < 20) matches = true;

            card.classList.toggle('hidden', !matches);
        });
    }
});
//account-slidshow
document.addEventListener('DOMContentLoaded', function () {
    const track = document.getElementById('offersTrack');
    const prevBtn = document.getElementById('offersPrev');
    const nextBtn = document.getElementById('offersNext');

    if (!track || !prevBtn || !nextBtn) return;

    const scrollAmount = 344;

    nextBtn.addEventListener('click', function () {
        const maxScroll = track.scrollWidth - track.clientWidth;
        if (track.scrollLeft >= maxScroll - 5) {
            track.scrollTo({ left: 0, behavior: 'smooth' });
        } else {
            track.scrollBy({ left: scrollAmount, behavior: 'smooth' });
        }
    });

    prevBtn.addEventListener('click', function () {
        if (track.scrollLeft <= 5) {
            track.scrollTo({ left: track.scrollWidth, behavior: 'smooth' });
        } else {
            track.scrollBy({ left: -scrollAmount, behavior: 'smooth' });
        }
    });
});