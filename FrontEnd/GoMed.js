document.addEventListener("DOMContentLoaded", function () {
    const bar = document.getElementById("bar");
    const nav = document.querySelector("nav ul");
    bar.addEventListener("click", function () {
        nav.classList.toggle("active");
    });

    const links = document.querySelectorAll("nav ul li a");
    links.forEach((link) => {
        link.addEventListener("click", function (e) {
            e.preventDefault();
            const href = this.getAttribute("href");
            const offsetTop = document.querySelector(href).offsetTop;
            scroll({
                top: offsetTop,
                behavior: "smooth"
            });
        });
    });

    const patientReviews = document.querySelectorAll(".patientReview");
    const detail = document.querySelector(".detail");
    const closeBtn = document.getElementById("closeBtn");
    const content = document.querySelector(".detail .content");
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
});