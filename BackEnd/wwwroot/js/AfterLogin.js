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
    const appointmentsButton = document.getElementById('appointmentsButton');
    const showmoreButton = document.getElementById('showMoreButton');
    const changePasswordButton = document.getElementById("changePasswordButton");
    const deleteUserButton = document.getElementById("deleteUserButton");
    appointmentsButton.addEventListener('click', (event) => {
        event.preventDefault();
        window.location.href = "/Patient/Appointments";
    });
    changePasswordButton.addEventListener('click', (event) => {
        event.preventDefault();
        window.location.href = "/Patient/ChangePassword";
    });
    deleteUserButton.addEventListener('click', (event) => {
        event.preventDefault();
        window.location.href = "/Patient/DeleteUser"
    })

    showmoreButton.addEventListener('click', (event) => {
        event.preventDefault();
        window.location.href = "/ShowMore/ShowMore";
    });
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
    document.getElementById("appointmentsButton").addEventListener("click", function () {
        fetch('/Patient/Appointments', {
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
    document.getElementById("changePasswordButton").addEventListener("click", function () {
        fetch('/Patient/ChangePassword', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.text();
            })
            .then(response => {
                if (data.success) {
                    console.log("Password updated succesfully");
                    window.location.href = "/Login";
                } else {
                    console.error("Error updating password:", data.errors);
                }

            }).catch(error => {
                console.error('There was a problem with your fetch operation:', error);
            });
    });
    document.getElementById("deleteUserButton").addEventListener("click", function () {
        fetch('/Patient/DeleteUser', {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                if (data.success) {
                    console.log("Deleted Successfully");
                    window.location.href = "/Home/Index";
                } else {
                    console.error("Error deleting user:", data.errors);
                }
            })
            .catch(error => {
                console.error('There was a problem with your fetch operation:', error);
            });
    });
});