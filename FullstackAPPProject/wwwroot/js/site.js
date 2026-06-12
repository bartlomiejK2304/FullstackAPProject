
document.addEventListener('DOMContentLoaded', function () {

    var hamburger = document.getElementById('hamburgerBtn');
    var navMenu = document.getElementById('navMenu');

    if (hamburger && navMenu) {
        hamburger.addEventListener('click', function () {
            navMenu.classList.toggle('aktywne');
           
            var ikona = hamburger.querySelector('i');
            if (navMenu.classList.contains('aktywne')) {
                ikona.className = 'bi bi-x-lg';
            } else {
                ikona.className = 'bi bi-list';
            }
        });

       
        var linki = navMenu.querySelectorAll('a');
        linki.forEach(function (link) {
            link.addEventListener('click', function () {
                navMenu.classList.remove('aktywne');
                var ikona = hamburger.querySelector('i');
                ikona.className = 'bi bi-list';
            });
        });
    }
    var dropdownBtn = document.getElementById('userDropdownBtn');
    var dropdownMenu = document.getElementById('userDropdownMenu');

    if (dropdownBtn && dropdownMenu) {
        dropdownBtn.addEventListener('click', function (e) {
            e.stopPropagation();
            dropdownMenu.classList.toggle('otwarte');
        });

        
        document.addEventListener('click', function () {
            dropdownMenu.classList.remove('otwarte');
        });

       
        dropdownMenu.addEventListener('click', function (e) {
            e.stopPropagation();
        });
    }

 
    var czat = document.querySelector('.czat');
    if (czat) {
        czat.scrollTop = czat.scrollHeight;
    }

    var inputyPlikow = document.querySelectorAll('input[type="file"][accept="image/*"]');
    inputyPlikow.forEach(function (input) {
        input.addEventListener('change', function (e) {
            var plik = e.target.files[0];
            if (!plik) return;

           
            if (!plik.type.startsWith('image/')) {
                alert('Wybrany plik nie jest obrazkiem!');
                input.value = '';
                return;
            }

           
            if (plik.size > 5 * 1024 * 1024) {
                alert('Plik jest za duży! Maksymalny rozmiar to 5MB.');
                input.value = '';
                return;
            }

         
            var staryPodglad = input.parentElement.querySelector('.podglad-zdjecia');
            if (staryPodglad) staryPodglad.remove();

         
            var reader = new FileReader();
            reader.onload = function (event) {
                var kontener = document.createElement('div');
                kontener.className = 'podglad-zdjecia';
                kontener.style.cssText = 'margin-top: 10px;';

                var img = document.createElement('img');
                img.src = event.target.result;
                img.style.cssText = 'max-width: 200px; border-radius: 8px; border: 1px solid #e2e8f0;';
                img.alt = 'Podgląd wybranego zdjęcia';

                var tekst = document.createElement('p');
                tekst.textContent = 'Podgląd: ' + plik.name;
                tekst.style.cssText = 'font-size: 12px; color: #64748b; margin: 4px 0 0 0;';

                kontener.appendChild(img);
                kontener.appendChild(tekst);
                input.parentElement.appendChild(kontener);
            };
            reader.readAsDataURL(plik);
        });
    });

    var textareas = document.querySelectorAll('textarea[maxlength]');
    textareas.forEach(function (textarea) {
        var max = parseInt(textarea.getAttribute('maxlength'));
        if (!max) return;

      
        var licznik = document.createElement('div');
        licznik.className = 'licznik-znakow';
        licznik.style.cssText = 'text-align: right; font-size: 12px; color: #64748b; margin-top: 4px;';
        licznik.textContent = textarea.value.length + ' / ' + max;
        textarea.parentElement.appendChild(licznik);

        
        textarea.addEventListener('input', function () {
            var aktualna = textarea.value.length;
            licznik.textContent = aktualna + ' / ' + max;

           
            if (aktualna > max * 0.9) {
                licznik.style.color = '#dc2626';
            } else {
                licznik.style.color = '#64748b';
            }
        });
    });

   
    var formyUsuwania = document.querySelectorAll('form[method="post"] .button-czerwony');
    formyUsuwania.forEach(function (przycisk) {
        if (!przycisk.hasAttribute('onclick')) {
            przycisk.addEventListener('click', function (e) {
                if (!confirm('Czy na pewno chcesz to usunąć?')) {
                    e.preventDefault();
                }
            });
        }
    });

    if ('IntersectionObserver' in window) {
        var karty = document.querySelectorAll('.karta, .kategoria-kafelek, .statystyka');
        karty.forEach(function (karta) {
            karta.style.opacity = '0';
            karta.style.transform = 'translateY(20px)';
            karta.style.transition = 'opacity 0.4s ease, transform 0.4s ease';
        });

        var observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    entry.target.style.opacity = '1';
                    entry.target.style.transform = 'translateY(0)';
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.1 });

        karty.forEach(function (karta) {
            observer.observe(karta);
        });
    }

 
    var formularze = document.querySelectorAll('form[method="post"]');
    formularze.forEach(function (form) {
        form.addEventListener('submit', function (e) {
            var pola = form.querySelectorAll('input[required], textarea[required]');
            var jestBlad = false;

            pola.forEach(function (pole) {
              
                pole.style.borderColor = '';
                var staryBlad = pole.parentElement.querySelector('.js-blad');
                if (staryBlad) staryBlad.remove();

                if (!pole.value.trim()) {
                    jestBlad = true;
                    pole.style.borderColor = '#dc2626';

                    var komunikat = document.createElement('span');
                    komunikat.className = 'js-blad';
                    komunikat.textContent = 'To pole jest wymagane';
                    komunikat.style.cssText = 'color: #dc2626; font-size: 13px; display: block; margin-top: 4px;';
                    pole.parentElement.appendChild(komunikat);
                }
            });

            if (jestBlad) {
                e.preventDefault();
               
                var pierwszyBlad = form.querySelector('[style*="border-color: rgb(220, 38, 38)"]');
                if (pierwszyBlad) {
                    pierwszyBlad.scrollIntoView({ behavior: 'smooth', block: 'center' });
                    pierwszyBlad.focus();
                }
            }
        });
    });

   
    var linkiWewnetrzne = document.querySelectorAll('a[href^="#"]');
    linkiWewnetrzne.forEach(function (link) {
        link.addEventListener('click', function (e) {
            var cel = document.querySelector(link.getAttribute('href'));
            if (cel) {
                e.preventDefault();
                cel.scrollIntoView({ behavior: 'smooth' });
            }
        });
    });

});


