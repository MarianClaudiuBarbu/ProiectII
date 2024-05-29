document.addEventListener("DOMContentLoaded", function () {
    const bar = document.getElementById("bar");
    const nav = document.querySelector("nav ul");

    if (bar && nav) {
        bar.addEventListener("click", function () {
            nav.classList.toggle("active");
        });
    }

    const links = document.querySelectorAll("nav ul li a");
    links.forEach((link) => {
        link.addEventListener("click", function (e) {
            e.preventDefault();
            const href = this.getAttribute("href");
            console.log("Link clicked, href:", href); // Add this line to log the href attribute
            if (href && href !== "#" && document.querySelector(href)) {
                const offsetTop = document.querySelector(href).offsetTop;
                scroll({
                    top: offsetTop,
                    behavior: "smooth"
                });
            } else {
                console.error("Invalid href or element not found:", href);
            }
        });
    });

    const patientReviews = document.querySelectorAll(".patientReview");
    const detail = document.querySelector(".detail");
    const closeBtn = document.getElementById("closeBtn");
    const content = document.querySelector(".detail .content");

    if (detail && closeBtn && content) {
        patientReviews.forEach((review) => {
            review.addEventListener("click", function () {
                const reviewText = this.querySelector(".info p").innerText;
                content.innerHTML = `<p>${reviewText}</p>`;
                detail.style.display = "flex";
            });
        });

        closeBtn.addEventListener("click", function () {
            detail.style.display = "none";
        });
    }

    const currentView = window.location.pathname.split('/').pop();
    console.log("Current view:", currentView);

    if (currentView === 'Login.cshtml') {
        const loginForm = document.getElementById('loginForm');
        if (loginForm) {
            loginForm.addEventListener('submit', async (event) => {
                event.preventDefault();

                const email = document.getElementById('loginEmail').value;
                const password = document.getElementById('loginPassword').value;

                try {
                    await login(email, password);
                } catch (error) {
                    console.error('Login failed:', error.message);
                    const errorMessageElement = document.getElementById('loginErrorMessage');
                    if (errorMessageElement) {
                        errorMessageElement.textContent = error.message;
                    }
                }
            });
        }
    } else if (currentView === 'Register.cshtml') {
        const registerForm = document.getElementById('registerForm');
        if (registerForm) {
            registerForm.addEventListener('submit', async (event) => {
                event.preventDefault();

                const formData = {
                    email: document.getElementById('registerEmail').value,
                    password: document.getElementById('registerPassword').value,
                    name: document.getElementById('registerName').value
                };

                try {
                    await register(formData);
                } catch (error) {
                    console.error('Registration failed:', error.message);
                    const errorMessageElement = document.getElementById('registerErrorMessage');
                    if (errorMessageElement) {
                        errorMessageElement.textContent = error.message;
                    }
                }
            });
        }
    }
    document.getElementById("showMoreButton").addEventListener("click", function () {
        fetch('/Home/ShowMore', {
            method: 'GET',
            headers: {
                'Content-Type': 'text/html'
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.text();
            })
            .then(html => {
                document.getElementById('container').innerHTML = html;
            })
            .catch(error => {
                console.error('There was a problem with your fetch operation:', error);
            });
    });
    document.getElementById("registerForm").addEventListener("submit", function (event) {
        event.preventDefault();

        const formData = new FormData(this);
        const jsonData = JSON.stringify(Object.fromEntries(formData.entries()));

        fetch('/Patient/Register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: jsonData
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.text();
            })
            .then(html => {
                document.getElementById('container').innerHTML = html;
            })
            .catch(error => {
                console.error('There was a problem with your fetch operation:', error);
            });
    });

});

async function login(email, password) {

    console.log("Logging in with", email, password);
}

async function register(formData) {

    console.log("Registering with", formData);
}
