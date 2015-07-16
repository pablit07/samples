from django import template

register = template.Library()


@register.filter
def redfill(percent):
    """
        This is a templatefilter for use in Django templates in the 'style' attribute of an element
        When given a percentage, it spits out the CSS color attribute for a particular hue of red
    """

    # this assumes we're using just for the rating remainder
    percent = float((percent % 1)) * 100

    # the max red color value
    r_max = 204.0
    color_max = 255.0

    r_rate = -1.0 * (color_max - r_max) / 100
    gb_rate = -1.0 * (color_max / 100)

    r = int(round(color_max + (r_rate * percent), 0))
    g = int(round(color_max + (gb_rate * percent), 0))
    b = g

    return "color:rgb({r}, {g}, {b})".format(r=r, g=g, b=b)