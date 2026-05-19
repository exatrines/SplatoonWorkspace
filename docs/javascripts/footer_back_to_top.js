function scrollToTop() {
  const topButton = document.querySelector("[data-md-component=top]");
  if (topButton) {
    topButton.click();
    return;
  }

  const main = document.querySelector(".md-main");
  if (main) {
    main.scrollTo({ top: 0, behavior: "smooth" });
  }

  window.scrollTo({ top: 0, behavior: "smooth" });
}

function bindFooterBackToTop() {
  document.querySelectorAll(".md-footer__back-to-top").forEach((link) => {
    if (link.dataset.bound) {
      return;
    }

    link.dataset.bound = "true";
    link.addEventListener("click", (event) => {
      event.preventDefault();
      scrollToTop();
    });
  });
}

if (typeof document$ !== "undefined") {
  document$.subscribe(bindFooterBackToTop);
} else {
  document.addEventListener("DOMContentLoaded", bindFooterBackToTop);
}
